//  Copyright 2007-2008 USFS Northern Research Station, Conservation Biology Institute, University of Wisconsin
//  Authors:  
//      Robert M. Scheller
//      Brian R. Miranda
//  License:  Available at  
//  http://www.landis-ii.org/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

namespace Landis.Fuels
{
    public class DeadCohorts
    {
        public int time;
        public int numCohorts;
        
        public DeadCohorts(int time, int numCohorts)
        {
            this.time = time;
            this.numCohorts = numCohorts;
        }
    }
}
