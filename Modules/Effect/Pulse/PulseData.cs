﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using CommonElements.ColorManagement.ColorModels;
using Vixen.Module;
using Vixen.Commands.KnownDataTypes;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;

namespace VixenModules.Effect.Pulse
{
	[DataContract]
	public class PulseData : ModuleDataModelBase
	{
		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public ColorGradient ColorGradient { get; set; }

		public override IModuleDataModel Clone()
		{
			PulseData result = new PulseData();
			result.LevelCurve = LevelCurve;
			result.ColorGradient = ColorGradient;
			return result;
		}
	}
}