using System;
using System.Collections.Generic;

namespace ApiServices.Models.RankedMatch
{
    [Serializable]
    public class CreateRankedMatchPayload
    {
        public CreateRankedMatchPayload(List<List<string>> teams)
        {
            this.teams = teams;
        }
        
        public List<List<string>> teams;
    }
}