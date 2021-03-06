﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using MikuBot.Commands;
using MikuBot.ExtraPlugins.Helpers;
using MikuBot.Helpers;
using MikuBot.LinkParsing;
using MikuBot.Modules;

namespace MikuBot.ExtraPlugins
{
	public class GelbooruParser : MsgCommandModuleBase
	{
		private readonly Dictionary<char, string> ratingNames = new Dictionary<char, string> {
			{ 's', "Safe" },
			{ 'q', "Questionable" },
			{ 'e', "Explicit" }
		};

		private readonly RegexLinkMatcher[] linkMatchers = new[] {
			new RegexLinkMatcher("www.gelbooru.com/index.php?page=post&s=view&id={0}", @"www\.gelbooru\.com/index.php\?page=post\&s=view\&id=(\d+)"),
		};

		private void HandlePage(Receiver receiver, int id)
		{
			var apiUrl = string.Format("http://www.gelbooru.com/index.php?page=dapi&s=post&q=index&id={0}", id);

			var request = (HttpWebRequest)WebRequest.Create(apiUrl);
			request.UserAgent = "MikuBot";
			XDocument doc;

			try
			{
				using (var response = request.GetResponse())
				using (var stream = response.GetResponseStream())
				{
					try
					{
						doc = XDocument.Load(stream);
					}
					catch (XmlException)
					{
						receiver.Msg("Gelbooru (error): Invalid response");
						return;
					}
				}
			}
			catch (WebException x)
			{
				receiver.Msg("Gelbooru (error): " + x.Message);
				return;
			}

			var res = doc.Element("posts");

			if (res == null || res.Element("post") == null)
			{
				receiver.Msg("Gelbooru (error): Invalid response");
				return;
			}

			var post = res.Element("post");

			var rating = post.Attribute("rating").Value;
			string ratingName;
			ratingNames.TryGetValue(rating[0], out ratingName);

			var width = post.Attribute("width").Value;
			var height = post.Attribute("height").Value;

			var createdAt = post.Attribute("created_at").Value;

			receiver.Msg(string.Format("Gelbooru: rating '{0}', {1}x{2} pixels, uploaded at {3}", ratingName, width, height, createdAt));
		}

		public override string HelpText
		{
			get { return "Parses Gelbooru links"; }
		}

		public override InitialModuleStatus InitialStatus
		{
			get { return InitialModuleStatus.Enabled; }
		}

		public override bool IsPassive
		{
			get { return true; }
		}

		public override string Name
		{
			get { return "Gelbooru"; }
		}

		public override void HandleCommand(MsgCommand cmd, IBotContext bot)
		{
			if (cmd.BotCommand.Is("NoLink"))
				return;

			var receiver = new Receiver(bot.Writer, cmd.ChannelOrSenderNick);
			var possibleUrl = cmd.Text;
			var matcher = linkMatchers.FirstOrDefault(m => m.IsMatch(possibleUrl));

			if (matcher == null)
				return;

			var url = PluginHelper.MakeLink(matcher.MakeLink(possibleUrl));
			var id = int.Parse(matcher.GetId(url));

			//GetPageContent(receiver, url);							// Synchronized version
			Task.Factory.StartNew(() => HandlePage(receiver, id))   // Async version
				.ContinueWith(TaskHelper.HandleTaskException, TaskContinuationOptions.OnlyOnFaulted);
		}

		public override void OnLoaded(IBotContext bot, IModuleFile moduleFile)
		{
		}
	}
}
