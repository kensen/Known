﻿using System.Collections.Generic;
using System.IO;

namespace Known.Cells
{
    public interface IExcel
    {
        IList<ISheet> Sheets { get; }

        void Open(string fileName);
        void Open(Stream stream);
        ISheet AddSheet(string name);
        void DeleteSheet(string name);
        void Save(string fileName);
        void Save(string fileName, SavedFormat format);
        MemoryStream SaveToStream();
        MemoryStream SaveToStream(SavedFormat format);
        void CalculateFormula();
    }
}
