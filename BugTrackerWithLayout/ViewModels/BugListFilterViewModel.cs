using System;
using System.Collections.Generic;
using System.Web.Mvc;
using PagedList;
using BugTrackerWithLayout.Models;

namespace BugTrackerWithLayout.ViewModels
{
    public class BugListFilterViewModel
    {
        // --- Filtreler ---
        public string Keyword { get; set; }          // Başlık/Açıklama araması
        public string Status { get; set; }           // Açık / Devam Ediyor / Çözüldü
        public string Priority { get; set; }         // Düşük / Orta / Yüksek
        public int? CategoryId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        //  Yeni: Tamamlanan / Tamamlanmayan filtresi
        public bool? IsCompleted { get; set; }       // null = tümü, true = tamamlanan, false = tamamlanmayan

        // --- Sayfalama ---
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;      // 5 / 10 / 20 / 50 / 99999(Hepsi)

        // --- Dropdown verileri (View için) ---
        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Statuses { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Priorities { get; set; } = new List<SelectListItem>();

        //  Yeni: Tamamlanma durumu dropdown listesi
        public IEnumerable<SelectListItem> CompletionStatuses { get; set; } = new List<SelectListItem>();

        // --- Sonuçlar ---
        public IPagedList<Bug> PagedBugs { get; set; }                 // Ekranda gösterilecek sayfalanmış liste
        public IEnumerable<Bug> AllBugsForExport { get; set; }         // Export için filtreli tam liste
    }
}
