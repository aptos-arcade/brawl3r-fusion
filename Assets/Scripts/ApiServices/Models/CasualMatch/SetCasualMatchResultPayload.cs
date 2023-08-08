using System;
using System.Collections.Generic;

namespace ApiServices.Models.CasualMatch
{
    [Serializable]
    public class SetCasualMatchResultPayload
    {
        public SetCasualMatchResultPayload(string matchId, int winnerIndex, List<List<CasualMatchPlayer>> teams)
        {
            this.matchId = matchId;
            this.winnerIndex = winnerIndex;
            this.teams = teams;
        }

        public string matchId;
        public int winnerIndex;
        public List<List<CasualMatchPlayer>> teams;
    }
}