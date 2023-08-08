using System;
using System.Collections.Generic;

namespace ApiServices.Models.CasualMatch
{
    [Serializable]
    public class CreateCasualMatchPayload
    {
        public CreateCasualMatchPayload(List<List<CasualMatchPlayer>> teams)
        {
            this.teams = teams;
        }

        public List<List<CasualMatchPlayer>> teams;
    }
}