using Landis.Species;
using Landis.AgeCohort;
using Landis.Cohorts;
using Landis.Landscape;
using Edu.Wisc.Forest.Flel.Util;
using System;
using System.Collections.Generic;


namespace Landis.Harvest
{

    public class InclusionRequirement
        : IRequirement
    {
	
		//list or rules for this inclusion requirement
		List<InclusionRule> rule_list = new List<InclusionRule>();

        //---------------------------------------------------------------------

		/// <summary>
		/// constructor, assigns the list of inclusion rules
		/// </summary>
		
        public InclusionRequirement(List<InclusionRule> rule_list) {
			//assign the list of inclusion rules
			this.rule_list = rule_list;
		}

        //---------------------------------------------------------------------
		
		/// <summary>
		/// use combination of all rules in rule_list to determine if this stand meets this inclusion requirement
		/// </summary>
        bool IRequirement.MetBy(Stand stand) {
			//UI.WriteLine("STAND = {0}", stand.MapCode);
			//make dictionary (key (found in get_key function for rule), val = count) of all species-age cohorts on this stand
			Dictionary<string, int> stand_cohorts = new Dictionary<string, int>();
			//fill this dictionary with all cohort data from this stand (returns whether or not there is any data in the dictionary)
			init_dictionary(stand, stand_cohorts);
			
			//flag for required (if at least one required is met and there are no violations will return true)
			//bool required_flag = false;
			//flags for optional (if at least one is met will return true)
			bool optional_flag = false;
			bool optional_met = false;
			//loop through each rule checking it against this stand
			foreach (InclusionRule rule in rule_list) {
				//build key-start key from species list
				string key_start = "";
				foreach (string species in rule.SpeciesList) {
					key_start += species + " ";
				}
				//check if the key for this rule exists.  if it does then there must be an entry.  if it doesn't then there are no entries
				try {
 					/* UI.WriteLine("RULE TYPE = {0}\nRULE AGE RANGE = {1} - {2}\nRULE PERCENT = {3}\nRULE KEY = {4}\nNUMBER OF COHORTS = {5}\nSTAND SITE COUNT = {6}",
					rule.InclusionType,
					rule.RuleAgeRange.Start, 
					rule.RuleAgeRange.End,
					rule.PercentOfCells, 
					get_key(key_start, rule.RuleAgeRange), 
					stand_cohorts[get_key(key_start, rule.RuleAgeRange)], 
					stand.SiteCount); */

					//boolean for condition checking
					bool meets = false;
					
					//assign a bool for the generic condition: there are enough of this rule's species in the stand.
					meets = check_rule(stand, stand_cohorts, rule, get_key(key_start, rule.RuleAgeRange), stand.SiteCount, rule.PercentOfCells);
//					UI.WriteLine("MEETS = {0}\n", meets);
					//forbidden
					if (rule.InclusionType == "Forbidden") {
						if (meets) {
							//this stand meets a 'forbidden' rule so it is disqualified (can return false right away)
//							UI.WriteLine("returning false1");
							return false;
						}
					}
					//required
					else if (rule.InclusionType == "Required") {
						if (!meets) {
							//this stand violates a 'required' rule so it is disqualified (can return false right away)
							//UI.WriteLine("returning false2");
							return false;
						}
						else {
							//this stand does not violate this 'required' rule.  in case of no optionals, 
							//it must know that it has fulfilled a required rule to return true.
//							UI.WriteLine("returning true1");
							//required_flag = true;
						}
					}
					//optional
					else if (rule.InclusionType == "Optional") {
						//mark that there has been an optional rule even included in the requirement
						optional_flag = true;
						if (meets) {
							//this stand met an 'optional' rule so it is still qualified (but can't return anything because it
							//may not be done checking all rules against this stand)
							//however, mark that at least 1 optional rule is fulfilled
							optional_met = true;
						}
					}
				}
				//do nothing if key is not found.
				catch (KeyNotFoundException) {
				}
			}

			
			if (optional_flag && !optional_met) {
				return false;
			}
			else if ((optional_flag && optional_met) || !optional_flag) {
				return true;
			}
//			UI.WriteLine("returning default truth!!");
			return true;
		}
		
        //---------------------------------------------------------------------
		/// <summary>
		/// initialize the cohort dictionary for this stand's evaluation
		/// </summary>		
		
		private void init_dictionary(Stand stand, Dictionary<string, int> stand_cohorts) {
			stand_cohorts.Clear();
			List<ActiveSite> sites = stand.GetSites();
			
			//loop through the sites gathering the species (only the ones involved in the rule_list) into a dictionary.
			//count the number of occurrences for each species-age pairing.
			
			foreach (ActiveSite site in sites) {
				//tally the occurrences of the cohorts:
				//if the cohort is the correct species and within the correct age range
				AgeCohort.ISiteCohorts site_cohorts = (AgeCohort.ISiteCohorts) Model.Core.SuccessionCohorts[site];
				foreach (InclusionRule rule in rule_list) {
					//part of the dictionary's key will be a grouping of the species names listed
					string key_start = "";
					foreach(string species in rule.SpeciesList) {
						key_start += species + " ";
					}
					//flag for if a species matching the age requirements (for this rule) is found in the site or not
					bool found_one = false;
					//get species cohorts by using this rule's species_indecis (in case more than 1 species is listed)
					//loop through all species indecis, will build dictionary
					foreach (int index in rule.SpeciesIndexList) {
						AgeCohort.ISpeciesCohorts species_cohorts = site_cohorts[Model.Core.Species[index]];
						
						//get enumeration of the cohorts if the species_cohorts is not null
						if (species_cohorts != null) {
							IEnumerator<AgeCohort.ICohort> cohort_enum = species_cohorts.GetEnumerator();

							//loop while you still can (will return false when it is at the end of the enum)
							while (cohort_enum.MoveNext()) {
								AgeCohort.ICohort cohort = cohort_enum.Current;
								//if the cohort fits the rule, tally it in the dictionary.
								if (rule.SpeciesList.Contains(cohort.Species.Name) && rule.RuleAgeRange.Contains(cohort.Age)) {
									try {
										// add this entry to the dictionary with a count of 1
										stand_cohorts.Add(get_key(key_start, rule.RuleAgeRange), 1);
									}
									// if there is an argument exception, then must increment hit-count for this cohort
									catch(ArgumentException) {
										// increment value for this cohort
										stand_cohorts[get_key(key_start, rule.RuleAgeRange)]++;
									}
									//break out of loop because this site has shown that it contains a cohort matching that rule.
									//also note that it should break out of the rule because it has already been met.
									found_one = true;
									break;
								}
							}
						}
						//break out of this loop if a species matching age requirements (for this rule)  has already been found in this site
						if (found_one) {
							break;
						}
					}
				}
			}
		}

		
        //---------------------------------------------------------------------
		
		/// <summary>
		/// make a string key for the dictionary.
		/// format is <species> <age range start> <age range end>
		/// </summary>
		
		private string get_key(string key_start, AgeRange age_range) {
			//the key is formed by concatenating the key_start (species list) with the age range parameters
			string key = key_start + age_range.Start + " " + age_range.End;
			return key;
		}
		
        //---------------------------------------------------------------------
		
		/// <summary>
		/// calculate the boolean expression for the given rule and stand parameters.
		/// </summary>
		/// <returns>
		/// the truth value.
		/// </returns>
		
		private bool check_rule(Stand stand, Dictionary<string, int> stand_cohorts, InclusionRule rule, string key, int site_count, double percent) {
			bool meets = false;
			//if percent is a value (-1 is number for 'highest') then return truth as normal calculation
			if (percent != -1) {
				meets = (stand_cohorts[key] > site_count * percent);
			}
			//if percent != -1, use 'highest' evaluation algorithm
			else {
				//compare the number of cohorts covered by this rule (found in stand_cohorts[key]) to all other species (for all their cohorts)

				//dictionary for other species
				Dictionary<string, int> other_species = new Dictionary<string, int>();
				List<ActiveSite> sites = stand.GetSites();
				//fill this other dictionary, grouped by other species in the stand
				foreach (ActiveSite site in sites) {
					Cohorts.TypeIndependent.ISiteCohorts cohorts = Model.Core.SuccessionCohorts[site];
					IList<ISpecies> species_present = cohorts.SpeciesPresent;
					//loop through these species names.  if it is a species name used in this rule, ignore it.
					//if it is not a species name used in this rule, tally it in the dictionary
					foreach (ISpecies species in species_present) {
						//UI.WriteLine("new species:");
						if (!rule.SpeciesList.Contains(species.Name)) {
							try {
								// add this entry to the dictionary with a count of 1
								other_species.Add(species.Name, 1);
								//UI.WriteLine("found species {0}", species.Name);
							}
							// if there is an argument exception, then must increment hit-count for this cohort
							catch (ArgumentException) {
								other_species[species.Name]++;
								//UI.WriteLine("incrementing {0} to {1}", species.Name, other_species[species.Name]);
							}
						}
					}
				}
				
				//now check whole dictionary against rule
				foreach (KeyValuePair<string, int> kvp in other_species) {
					//if there is another species that appears more often than this grouping of cohorts (defined by the rule)
					//then the rule fails.
					if (stand_cohorts[key] < kvp.Value) {
						//UI.WriteLine("FAILURE: {0} has {1} and {2} only has {3}", kvp.Key, kvp.Value, key, stand_cohorts[key]); 
						return false;
					}
					//for testing only
					/* else {
						UI.WriteLine("SUCCESS: {0} has {1} and {2} only has {3}", key, stand_cohorts[key], kvp.Key, kvp.Value); 
					} */
				}
				//if the loop went all the way through, then the rule is met, return true
				meets = true;
			}
			//return truth value
			return meets;
		}
    }
}

		

