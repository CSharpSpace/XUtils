using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace XUtils.Schedule
{
	public static class ScheduledUtils
	{
		private const string pattern = "\\[(?<Type>[a-zA-Z]+)\\{(?<Offset>(\\d+\\/)?(\\d+\\,+)?((\\d+:){2}\\d+\\.)?\\d+)\\}(\\{(?<Count>\\d+)\\})?(\\{(?<EndDate>(((\\d+(-|/)){2}\\d+\\s(\\d+:){2}\\d+)))\\})?\\]";
		private const string patternBlockInterval = "\\[\\{(?<Begin>(((\\d+(-|/)){2}\\d+\\s(\\d+:){2}\\d+)))\\}\\{(?<Interval>(\\d+\\/)?(\\d+\\,+)?((\\d+:){2}\\d+\\.)?\\d+)\\}\\{(?<RegionType>[a-zA-Z]+)\\{(?<StartOffset>(\\d+\\/)?(\\d+\\,+)?((\\d+:){2}\\d+\\.)?\\d+)\\}\\{(?<StopOffset>(\\d+\\/)?(\\d+\\,+)?((\\d+:){2}\\d+\\.)?\\d+)\\}\\}\\]";
		private const string patternBlockTimer = "\\[\\{(?<Begin>(((\\d+(-|/)){2}\\d+\\s(\\d+:){2}\\d+)))\\}\\{(?<RuleType>[a-zA-Z]+)\\{(?<RuleOffset>(\\d+\\/)?(\\d+\\,+)?((\\d+:){2}\\d+\\.)?\\d+)\\}\\}\\{(?<RegionType>[a-zA-Z]+)\\{(?<StartOffset>(\\d+\\/)?(\\d+\\,+)?((\\d+:){2}\\d+\\.)?\\d+)\\}\\{(?<StopOffset>(\\d+\\/)?(\\d+\\,+)?((\\d+:){2}\\d+\\.)?\\d+)\\}\\}\\]";
		private static Regex regex = new Regex("\\[(?<Type>[a-zA-Z]+)\\{(?<Offset>(\\d+\\/)?(\\d+\\,+)?((\\d+:){2}\\d+\\.)?\\d+)\\}(\\{(?<Count>\\d+)\\})?(\\{(?<EndDate>(((\\d+(-|/)){2}\\d+\\s(\\d+:){2}\\d+)))\\})?\\]", RegexOptions.Compiled);
		private static Regex regexBlockInterval = new Regex("\\[\\{(?<Begin>(((\\d+(-|/)){2}\\d+\\s(\\d+:){2}\\d+)))\\}\\{(?<Interval>(\\d+\\/)?(\\d+\\,+)?((\\d+:){2}\\d+\\.)?\\d+)\\}\\{(?<RegionType>[a-zA-Z]+)\\{(?<StartOffset>(\\d+\\/)?(\\d+\\,+)?((\\d+:){2}\\d+\\.)?\\d+)\\}\\{(?<StopOffset>(\\d+\\/)?(\\d+\\,+)?((\\d+:){2}\\d+\\.)?\\d+)\\}\\}\\]", RegexOptions.Compiled);
		private static Regex regexBlockTimer = new Regex("\\[\\{(?<Begin>(((\\d+(-|/)){2}\\d+\\s(\\d+:){2}\\d+)))\\}\\{(?<RuleType>[a-zA-Z]+)\\{(?<RuleOffset>(\\d+\\/)?(\\d+\\,+)?((\\d+:){2}\\d+\\.)?\\d+)\\}\\}\\{(?<RegionType>[a-zA-Z]+)\\{(?<StartOffset>(\\d+\\/)?(\\d+\\,+)?((\\d+:){2}\\d+\\.)?\\d+)\\}\\{(?<StopOffset>(\\d+\\/)?(\\d+\\,+)?((\\d+:){2}\\d+\\.)?\\d+)\\}\\}\\]", RegexOptions.Compiled);
		private static Type type = typeof(EventTimeEnums);
		public static IList<IScheduledRule> LoadRules(string strRules)
		{
			IList<IScheduledRule> list = new List<IScheduledRule>();
			foreach (Match match in ScheduledUtils.regex.Matches(strRules))
			{
				ScheduledRule scheduledRule = new ScheduledRule();
				scheduledRule.Rule = new TaskRule
				{
					Type = match.Groups["Type"].Value,
					Offest = match.Groups["Offset"].Value
				};
				string value = match.Groups["Count"].Value;
				scheduledRule.Count = (string.IsNullOrEmpty(value) ? 0 : TypeParsers.ConvertTo<int>(value));
				string value2 = match.Groups["EndDate"].Value;
				scheduledRule.End = (string.IsNullOrEmpty(value2) ? DateTime.MinValue : TypeParsers.ConvertTo<DateTime>(value2));
				if (Enum.IsDefined(ScheduledUtils.type, scheduledRule.Rule.Type))
				{
					list.Add(scheduledRule);
				}
			}
			foreach (Match match2 in ScheduledUtils.regexBlockInterval.Matches(strRules))
			{
				ScheduledBlockIntervalRule scheduledBlockIntervalRule = new ScheduledBlockIntervalRule();
				scheduledBlockIntervalRule.Begin = match2.Groups["Begin"].Value;
				scheduledBlockIntervalRule.Interval = match2.Groups["Interval"].Value;
				scheduledBlockIntervalRule.Region = new TaskRegion
				{
					Type = match2.Groups["RegionType"].Value,
					StartOffest = match2.Groups["StartOffset"].Value,
					StopOffest = match2.Groups["StopOffset"].Value
				};
				if (Enum.IsDefined(ScheduledUtils.type, scheduledBlockIntervalRule.Region.Type))
				{
					list.Add(scheduledBlockIntervalRule);
				}
			}
			foreach (Match match3 in ScheduledUtils.regexBlockTimer.Matches(strRules))
			{
				ScheduledBlockTimerRule scheduledBlockTimerRule = new ScheduledBlockTimerRule();
				scheduledBlockTimerRule.Begin = match3.Groups["Begin"].Value;
				scheduledBlockTimerRule.Rule = new TaskRule
				{
					Type = match3.Groups["RuleType"].Value,
					Offest = match3.Groups["RuleOffset"].Value
				};
				scheduledBlockTimerRule.Region = new TaskRegion
				{
					Type = match3.Groups["RegionType"].Value,
					StartOffest = match3.Groups["StartOffset"].Value,
					StopOffest = match3.Groups["StopOffset"].Value
				};
				if (Enum.IsDefined(ScheduledUtils.type, scheduledBlockTimerRule.Rule.Type) && Enum.IsDefined(ScheduledUtils.type, scheduledBlockTimerRule.Region.Type))
				{
					list.Add(scheduledBlockTimerRule);
				}
			}
			return list;
		}
	}
}
