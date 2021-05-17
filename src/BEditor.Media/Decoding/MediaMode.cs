﻿using System;

namespace BEditor.Media.Decoding
{
    /// <summary>
    /// Represents the audio/video streams loading modes.
    /// </summary>
    [Flags]
    public enum MediaMode
    {
        /// <summary>
        /// Enables loading only video streams.
        /// </summary>
        Video,

        /// <summary>
        /// Enables loading only audio streams.
        /// </summary>
        Audio,

        /// <summary>
        /// Enables loading both audio and video streams if they exist.
        /// </summary>
        AudioVideo,
    }
}