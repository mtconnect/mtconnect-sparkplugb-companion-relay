﻿using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace MTConnectSharp
{
   public interface IDataItem
   {
      int BufferSize { get; set; }
      DataItemSample PreviousSample { get; }
      DataItemSample CurrentSample { get; }
      ReadOnlyObservableCollection<DataItemSample> SampleHistory { get; }
      string Id { get; }
      string Name { get; }
      string LongName { get; }
      string Category { get; }
      string Type { get; }
      string SubType { get; }
      string Units { get; }
      string NativeUnits { get; }
      XElement Model { get; }
      void AddSample(DataItemSample newSample);
   }
}
