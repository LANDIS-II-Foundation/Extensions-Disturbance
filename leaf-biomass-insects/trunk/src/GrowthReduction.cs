//  Copyright 2006-2011 University of Wisconsin, Portland State University
//  Authors:  Jane Foster, Robert M. Scheller

//using Landis.Extension.Succession.Biomass;
using Landis.Core;
using Landis.SpatialModeling;
using Landis.Library.LeafBiomassCohorts;
using System.Collections.Generic;
using System;


namespace Landis.Extension.Insects
{
    public class GrowthReduction
    {

        private static IEnumerable<IInsect> manyInsect;
        //---------------------------------------------------------------------

        public static void Initialize(IInputParameters parameters)
        {
            manyInsect = parameters.ManyInsect;

            // Assign the method below to the CohortGrowthReduction delegate in
            // biomass-cohorts/Biomass.CohortGrowthReduction.cs
            CohortGrowthReduction.Compute = ReduceCohortGrowth;

        }


        //---------------------------------------------------------------------
        // This method replaces the delegate method.  It is called every year when
        // ACT_ANPP is calculated, for each cohort.  Therefore, this method is operating at
        // an ANNUAL time step and separate from the normal extension time step.

        public static double ReduceCohortGrowth(ICohort cohort, ActiveSite site)
        {
            //PlugIn.ModelCore.Log.WriteLine("   Calculating cohort growth reduction due to insect defoliation...");

            double summaryGrowthReduction = 0.0;
             
            //int siteBiomass = 0; //AMK Update from Jane's BiomassInsects

            //foreach (ISpeciesCohorts spp in SiteVars.Cohorts[site])
            //    foreach (ICohort spp_cohort in spp)
            //        siteBiomass += spp_cohort.Biomass;

            int sppIndex = cohort.Species.Index;

            foreach(IInsect insect in PlugIn.ManyInsect)
            {
                if(!insect.ActiveOutbreak)
                    continue;

                int suscIndex = insect.SppTable[sppIndex].Susceptibility - 1;
                
                //if (suscIndex < 0) 
                //    return 0.0;

                int yearBack = 0;
                double annualDefoliation = 0.0;

                if(insect.HostDefoliationByYear[site].ContainsKey(PlugIn.ModelCore.CurrentTime - yearBack))
                {
                    //PlugIn.ModelCore.Log.WriteLine("Host Defoliation By Year:  Time={0}, suscIndex={1}, spp={2}.", (PlugIn.ModelCore.CurrentTime - yearBack), suscIndex+1, cohort.Species.Name);
                    annualDefoliation += insect.HostDefoliationByYear[site][PlugIn.ModelCore.CurrentTime - yearBack][suscIndex];
                }
                double cumulativeDefoliation = annualDefoliation;

                while(annualDefoliation > 0)
                {
                    yearBack++;
                    annualDefoliation = 0.0;
                    if(insect.HostDefoliationByYear[site].ContainsKey(PlugIn.ModelCore.CurrentTime - yearBack))
                    {
                        //PlugIn.ModelCore.Log.WriteLine("Host Defoliation By Year:  Time={0}, suscIndex={1}, spp={2}.", (PlugIn.ModelCore.CurrentTime - yearBack), suscIndex+1, cohort.Species.Name);
                        annualDefoliation = insect.HostDefoliationByYear[site][PlugIn.ModelCore.CurrentTime - yearBack][suscIndex];
                        cumulativeDefoliation += annualDefoliation;
                    }
                }

                double slope = insect.SppTable[sppIndex].GrowthReduceSlope;
                double intercept = insect.SppTable[sppIndex].GrowthReduceIntercept;

                double growthReduction = 1.0 - (cumulativeDefoliation * slope + intercept);

                summaryGrowthReduction += growthReduction;
                //PlugIn.ModelCore.Log.WriteLine("Time={0}, Spp={1}, SummaryGrowthReduction={2:0.00}.", PlugIn.ModelCore.CurrentTime,cohort.Species.Name, summaryGrowthReduction);

            }
            if (summaryGrowthReduction > 1.0)  // Cannot exceed 100%
                summaryGrowthReduction = 1.0;

            if (summaryGrowthReduction < 0.0)  //AMK: if growth reduction <0 I told it to defaul to zero. Similar to >1.0
                summaryGrowthReduction = 0.0;

            //if(summaryGrowthReduction > 1.0 || summaryGrowthReduction < 0.0)  //AMK: this was the old version that triggered an error if reduction <0
            //{
            //    PlugIn.ModelCore.UI.WriteLine("Cohort Total Growth Reduction = {0:0.00}.  Site R/C={1}/{2}.", summaryGrowthReduction, site.Location.Row, site.Location.Column);
            //    throw new ApplicationException("Error: Total Growth Reduction is not between 1.0 and 0.0");
            //}
            
            //if(summaryGrowthReduction > 0.0 && PlugIn.ModelCore.CurrentTime > 8)
            //    PlugIn.ModelCore.Log.WriteLine("   Cohort growth reduction due to insect defoliation = {0:0.00}", summaryGrowthReduction);


            return summaryGrowthReduction;
        }



    }

}
