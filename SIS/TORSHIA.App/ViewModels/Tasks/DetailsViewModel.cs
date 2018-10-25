﻿using System;
using System.Collections.Generic;

namespace TORSHIA.App.ViewModels.Tasks
{
    public class DetailsViewModel
    {
        public string Title { get; set; }

        public int Level { get; set; }

        public string DueDate { get; set; }

        public string Description { get; set; }

        public string Participants { get; set; }

        public string AffectedSectors { get; set; }
    }
}
