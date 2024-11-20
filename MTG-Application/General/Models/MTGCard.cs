﻿using CommunityToolkit.Mvvm.ComponentModel;

namespace MTGApplication.General.Models;

public partial class MTGCard(MTGCardInfo info) : ObservableObject
{
  public MTGCardInfo Info { get; set { field = value; OnPropertyChanged(); } } = info;
}