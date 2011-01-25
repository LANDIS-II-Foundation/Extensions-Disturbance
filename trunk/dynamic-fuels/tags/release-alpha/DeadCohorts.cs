//  Copyright 2005 University of Wisconsin
//  Authors:  
//      Robert M. Scheller
//      James B. Domingo
//  BDA originally programmed by Wei (Vera) Li at University of Missouri-Columbia in 2004.
//  Version 1.0
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

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
