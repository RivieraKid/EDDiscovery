﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDDiscovery.EliteDangerous.JournalEvents
{
    //    When written: The player has been rewarded for a capital ship combat
    //Parameters:
    //•	Reward: value of award
    //•	AwardingFaction
    //•	VictimFaction

    public class JournalCapShipBond : JournalEntry
    {
        public JournalCapShipBond(JObject evt) : base(evt, JournalTypeEnum.CapShipBond)
        {
            AwardingFaction = Tools.GetStringDef(evt["AwardingFaction"]);
            VictimFaction = Tools.GetStringDef(evt["VictimFaction"]);
            Reward = Tools.GetInt(evt["Reward"]);
        }
        public string AwardingFaction { get; set; }
        public string VictimFaction { get; set; }
        public int Reward { get; set; }

    }
}