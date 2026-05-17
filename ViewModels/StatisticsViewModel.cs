using divelog.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace divelog.ViewModels
{
    public class StatisticsViewModel
    {
        public int SelectedYear { get; set; }

        public int? SelectedGroupId { get; set; }

        public int SelectedRoleId { get; set; }
        public string ActiveTab { get; set; } = "dives";

        public List<Group> Groups { get; set; } = [];

        public List<SelectListItem> Roles { get; set; } = [];

        public List<StatisticsRowViewModel> Rows { get; set; } = [];

        public List<DivePurposeStatisticsRowViewModel> PurposeRows { get; set; } = [];
    }
}