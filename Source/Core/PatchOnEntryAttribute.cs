using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace ClosingBattle.Core;

/// <summary>
///     Marker attribute to indicate that a patch class will be applied in the Plugin.Awake method.
/// </summary>
[PublicAPI]
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
internal sealed class PatchOnEntryAttribute : Attribute;