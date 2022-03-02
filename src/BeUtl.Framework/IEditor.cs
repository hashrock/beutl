﻿using Avalonia.Controls;

namespace BeUtl.Framework;

public interface IEditor : IControl
{
    ViewExtension Extension { get; }

    string EdittingFile { get; }

    void Close();
}
