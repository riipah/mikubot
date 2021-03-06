﻿using System;

namespace MikuBot.ExtraPlugins.Helpers
{
	public static class PluginHelper
	{
		public static string MakeLink(string partialLink, bool assumeWww = false)
		{
			if (partialLink.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) || partialLink.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
				return partialLink;

			if (assumeWww && !partialLink.StartsWith("www.", StringComparison.InvariantCultureIgnoreCase))
				return "http://www." + partialLink;

			return "http://" + partialLink;
		}
	}
}
