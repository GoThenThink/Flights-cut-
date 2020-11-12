using Business.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Flights.Business.Services.HelperMethods
{
    internal static class SearchFilterHelpers
    {
        internal static async Task FormatFilterValuesAsync(FlightSearchFilters filters)
        {
            await Task.Run(() => {
                if (string.IsNullOrWhiteSpace(filters.SearchByName))
                    filters.SearchByName = null;
                else filters.SearchByName = filters.SearchByName.ToLower();
                if (filters.Statuses.Equals(null) || filters.Statuses.Count.Equals(0))
                    filters.Statuses = null;
                if (filters.Airlines.Equals(null) || filters.Airlines.Count.Equals(0))
                    filters.Airlines = null;
            });
        }

        internal static async Task MarkSelectedNonEmptyFiltersAsync(FlightSearchFilters filters, FilterGroup fg)
        {
            await Task.Run(() => {

                if (filters.Statuses != null)
                {
                    var listForIsSelected = fg.Filters.Where(x => x.Count > 0 && x.FilterGroupName.Equals(nameof(filters.Statuses)) && filters.Statuses.Contains(x.Id)).ToList();
                    foreach (var filter in listForIsSelected)
                    {
                        filter.IsSelected = true;
                    }
                }

                if (filters.Airlines != null)
                {
                    var listForIsSelected = fg.Filters.Where(x => x.Count > 0 && x.FilterGroupName.Equals(nameof(filters.Airlines)) && filters.Airlines.Contains(x.Id)).ToList();
                    foreach (var filter in listForIsSelected)
                    {
                        filter.IsSelected = true;
                    }
                }
            });
        }
    }
}
