﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys.Attribute;
using Vixen.Sys.State.Execution;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Meteors
{
	public class Meteors : PixelEffectBase
	{
		private MeteorsData _data;
		private readonly List<MeteorClass> _meteors = new List<MeteorClass>();
		private static Random _random = new Random();
		private double _gradientPosition = 0;

		public Meteors()
		{
			_data = new MeteorsData();
		}

		#region Setup

		[Value]
		public override StringOrientation StringOrientation
		{
			get { return _data.Orientation; }
			set
			{
				_data.Orientation = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Config properties

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"MovementType")]
		[ProviderDescription(@"MovementType")]
		[PropertyOrder(1)]
		public MeteorsEffect MeteorEffect
		{
			get { return _data.MeteorEffect; }
			set
			{
				_data.MeteorEffect = value;
				IsDirty = true;
				UpdateDirectionAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Direction")]
		[ProviderDescription(@"Direction")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 360, 1)]
		[PropertyOrder(2)]
		public int Direction
		{
			get { return _data.Direction; }
			set
			{
				_data.Direction = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}
		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"MinAngle")]
		[ProviderDescription(@"MinMeteorAngle")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 360, 1)]
		[PropertyOrder(3)]
		public int MinDirection
		{
			get { return _data.MinDirection; }
			set
			{
				_data.MinDirection = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"MaxAngle")]
		[ProviderDescription(@"MaxMeteorAngle")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 360, 1)]
		[PropertyOrder(4)]
		public int MaxDirection
		{
			get { return _data.MaxDirection; }
			set
			{
				_data.MaxDirection = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}


		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Speed")]
		[ProviderDescription(@"Speed")]
		//[NumberRange(1, 200, 1)]
		[PropertyOrder(5)]
		public Curve CenterSpeedCurve
		{
			get { return _data.CenterSpeedCurve; }
			set
			{
				_data.CenterSpeedCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"SpeedVariation")]
		[ProviderDescription(@"SpeedVariation")]
		//[NumberRange(1, 200, 1)]
		[PropertyOrder(6)]
		public Curve SpeedVariationCurve
		{
			get { return _data.SpeedVariationCurve; }
			set
			{
				_data.SpeedVariationCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Count")]
		[ProviderDescription(@"Count")]
		//[NumberRange(1, 200, 1)]
		[PropertyOrder(7)]
		public Curve PixelCountCurve
		{
			get { return _data.PixelCountCurve; }
			set
			{
				_data.PixelCountCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"TailLength")]
		[ProviderDescription(@"TailLength")]
		//[NumberRange(1, 100, 1)]
		[PropertyOrder(8)]
		public Curve LengthCurve
		{
			get { return _data.LengthCurve; }
			set
			{
				_data.LengthCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"RandomPosition")]
		[ProviderDescription(@"RandomPosition")]
		[PropertyOrder(9)]
		public bool RandomMeteorPosition
		{
			get { return _data.RandomMeteorPosition; }
			set
			{
				_data.RandomMeteorPosition = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Color properties

		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"ColorType")]
		[ProviderDescription(@"ColorType")]
		[PropertyOrder(0)]
		public MeteorsColorType ColorType
		{
			get { return _data.ColorType; }
			set
			{
				_data.ColorType = value;
				IsDirty = true;
				UpdateColorAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"ColorGradients")]
		[ProviderDescription(@"Color")]
		[PropertyOrder(1)]
		public List<ColorGradient> Colors
		{
			get { return _data.Colors; }
			set
			{
				_data.Colors = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Level properties

		[Value]
		[ProviderCategory(@"Brightness", 3)]
		[ProviderDisplayName(@"RandomIntensity")]
		[ProviderDescription(@"RandomIntensity")]
		[PropertyOrder(0)]
		public bool RandomBrightness
		{
			get { return _data.RandomBrightness; }
			set
			{
				_data.RandomBrightness = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Brightness", 3)]
		[ProviderDisplayName(@"Brightness")]
		[ProviderDescription(@"Brightness")]
		[PropertyOrder(1)]
		public Curve LevelCurve
		{
			get { return _data.LevelCurve; }
			set
			{
				_data.LevelCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Update Attributes
		private void UpdateAttributes()
		{
			UpdateColorAttribute(false);
			UpdateDirectionAttribute(false);
			TypeDescriptor.Refresh(this);
		}

		//Used to hide Colors from user when Rainbow type is selected and unhides for the other types.
		private void UpdateColorAttribute(bool refresh = true)
		{
			bool meteorType = ColorType != MeteorsColorType.RainBow;
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1);
			propertyStates.Add("Colors", meteorType);
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		private void UpdateDirectionAttribute(bool refresh = true)
		{
			bool direction = false;
			bool variableDirection = false;
			if (MeteorEffect == MeteorsEffect.Explode)
			{
				direction = true;
			}
			if (MeteorEffect == MeteorsEffect.RandomDirection)
			{
				direction = true;
				variableDirection = true;
			}
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(3);
			propertyStates.Add("Direction", !direction);
			propertyStates.Add("MinDirection", variableDirection);
			propertyStates.Add("MaxDirection", variableDirection);
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		#endregion

		#region Information

		public override string Information
		{
			get { return "Visit the Vixen Lights website for more information on this effect."; }
		}

		public override string InformationLink
		{
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/meteors/"; }
		}

		#endregion

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as MeteorsData;
				UpdateAttributes();
				IsDirty = true;
			}
		}

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		protected override void SetupRender()
		{
			//Nothing to setup
		}

		protected override void CleanUpRender()
		{
			//Nothing to clean up
		}

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			if (frame == 0)
				_meteors.Clear();
			int colorcnt = Colors.Count();
			var intervalPos = GetEffectTimeIntervalPosition(frame);
			var intervalPosFactor = intervalPos * 100;
			var length = CalculateLength(intervalPosFactor);
			int tailLength = (BufferHt < 10) ? length / 10 : BufferHt * length / 100;
			int minDirection = 1;
			int maxDirection = 360;
			int pixelCount = CalculatePixelCount(intervalPosFactor);
			var centerSpeed = CalculateCenterSpeed(intervalPosFactor);
			var spreadSpeed = CalculateSpeedVariation(intervalPosFactor);
			var minSpeed = centerSpeed - (spreadSpeed / 2);
			var maxSpeed = centerSpeed + (spreadSpeed / 2);
			if (minSpeed < 1)
				minSpeed = 1;
			if (maxSpeed > 200)
				maxSpeed = 200;
			if (tailLength < 1) tailLength = 1;
			int tailStart = BufferHt;
			if (tailStart < 1) tailStart = 1;

			// create new meteors and maintain maximum number as per users selection.
			HSV hsv = new HSV();
			int adjustedPixelCount = frame < pixelCount
				? (!RandomMeteorPosition && frame > pixelCount ? 1 : (pixelCount < 10 ? pixelCount : pixelCount / 10))
				: pixelCount;

			for (int i = 0; i < adjustedPixelCount; i++)
			{
				double position = (double) _random.Next(minSpeed, maxSpeed + 1)/20;
				if (_meteors.Count >= pixelCount) continue;
				MeteorClass m = new MeteorClass();
				if (MeteorEffect == MeteorsEffect.RandomDirection)
				{
					minDirection = MinDirection;
					maxDirection = MaxDirection;
				}

				int direction;
				if (MeteorEffect == MeteorsEffect.None)
					direction = Direction; //Set Range for standard Meteor as we don't want to just have them going straight down or two dirctions like the original Meteor effect.
				else
				{
					//This is to generate random directions between the Min and Max values
					//However if Someone makes the MaxDirection lower then the Min Direction then
					//the new direction will be the inverserve of the Min and Max effectively changing
					//the range from a downward motion to an upward motion, increasing the feature capability.
					if (maxDirection <= minDirection)
					{
						//used for the Upward movement of the Meteor (added feature)
						direction = _random.Next(1, 3) == 1 ? _random.Next(1, maxDirection) : _random.Next(minDirection, 360);
					}
					else
					{
						//used for the downward movemnet of the Meteor (standard way)
						direction = _random.Next(minDirection, maxDirection);
					}
				}
				//Moving
				m.X = rand() % BufferWi;
				m.Y = rand() % BufferHt;
				if (direction >= 0 && direction <= 90)
				{
					m.TailX = ((double)direction / 90);
					m.DeltaX = m.TailX * position;
					m.TailY = ((double)Math.Abs(direction - 90) / 90);
					m.DeltaY = m.TailY * position;
					if (_random.NextDouble() >= (double)(90 - direction) / 100)
					{
						m.X = 0;
						m.Y = rand() % BufferHt;
					}
					else
					{
						m.X = rand()%BufferWi;
						m.Y = 0;
					}
				}
				else if (direction > 90 && direction <= 180)
				{
					m.TailX = ((double)Math.Abs(direction - 180) / 90);
					m.DeltaX = m.TailX * position;
					m.TailY = -1 * ((double)Math.Abs(direction - 90) / 90);
					m.DeltaY = m.TailY * position;
					if (_random.NextDouble() >= (double)(180 - direction) / 100)
					{
						m.X = rand() % BufferWi;
						m.Y = BufferHt;
					}
					else
					{
						m.X = 0;
						m.Y = rand() % BufferHt;
					}
				}
				else if (direction > 180 && direction <= 270)
				{
					m.TailX = -1 * ((double)Math.Abs(direction - 180) / 90);
					m.DeltaX = m.TailX * position;
					m.TailY = -1 * ((double)Math.Abs(direction - 270) / 90);
					m.DeltaY = m.TailY * position;
					if (_random.NextDouble() >= (double)(270 - direction) / 100)
					{
						m.X = BufferWi;
						m.Y = rand() % BufferHt;
					}
					else
					{
						m.X = rand() % BufferWi;
						m.Y = BufferHt;
					}
				}
				else if (direction > 270 && direction <= 360)
				{
					m.TailX = -1 * ((double)Math.Abs(direction - 360) / 90);
					m.DeltaX = m.TailX * position;
					m.TailY = ((double)Math.Abs(270 - direction) / 90);
					m.DeltaY = m.TailY * position;
					if (_random.NextDouble() >= (double)(360-direction)/100)
					{
						m.X = rand() % BufferWi;
						m.Y = 0;
					}
					else
					{
						m.X = BufferWi;
						m.Y = rand() % BufferHt;
					}
				}

				if (MeteorEffect == MeteorsEffect.Explode)
				{
					m.X = BufferWi/2;
					m.Y = BufferHt/2;
				}
				else
				{
					if (RandomMeteorPosition || frame < pixelCount)
					{
						m.X = rand() % BufferWi;
						m.Y = (BufferHt - 1 - (rand() % tailStart));
					}
				}
				m.DeltaXOrig = m.DeltaX;
				m.DeltaYOrig = m.DeltaY;

				switch (ColorType)
				{
					case MeteorsColorType.Range: //Random two colors are selected from the list for each meteor.
						m.Hsv =
							SetRangeColor(HSV.FromRGB(Colors[rand() % colorcnt].GetColorAt((intervalPosFactor) / 100)),
								HSV.FromRGB(Colors[rand() % colorcnt].GetColorAt((intervalPosFactor) / 100)));
						break;
					case MeteorsColorType.Palette: //All colors are used
						m.Hsv = HSV.FromRGB(Colors[rand() % colorcnt].GetColorAt((intervalPosFactor) / 100));
						break;
					case MeteorsColorType.Gradient:
						m.Color = rand() % colorcnt;
						_gradientPosition = 100 / (double)tailLength / 100;
						m.Hsv = HSV.FromRGB(Colors[m.Color].GetColorAt(0));
						break;
				}
				m.HsvBrightness = RandomBrightness ? _random.NextDouble() * (1.0 - .25) + .25 : 1;
				_meteors.Add(m);
			}

			// render meteors
			foreach (MeteorClass meteor in _meteors)
			{
				meteor.DeltaX += meteor.DeltaXOrig;
				meteor.DeltaY += meteor.DeltaYOrig;
				int colorX = (meteor.X + Convert.ToInt32(meteor.DeltaX) - (BufferWi / 100));
				int colorY = (meteor.Y + Convert.ToInt32(meteor.DeltaY) + (BufferHt / 100));

				for (int ph = 0; ph < tailLength; ph++)
				{
					switch (ColorType)
					{
						case MeteorsColorType.RainBow: //No user colors are used for Rainbow effect.
							meteor.Hsv.H = (float) (rand()%1000)/1000.0f;
							meteor.Hsv.S = 1.0f;
							meteor.Hsv.V = 1.0f;
							break;
						case MeteorsColorType.Gradient:
							meteor.Hsv = HSV.FromRGB(Colors[meteor.Color].GetColorAt(_gradientPosition*ph));
							break;
					}
					hsv = meteor.Hsv;
					hsv.V *= meteor.HsvBrightness;
					hsv.V *= (float) (1.0 - ((double) ph/tailLength)*0.75);
					//Adjusts the brightness based on the level curve
					hsv.V = hsv.V * LevelCurve.GetValue(intervalPosFactor) / 100;
					var decPlaces = (int) (((decimal) (meteor.TailX*ph)%1)*100);
					if (decPlaces <= 40 || decPlaces >= 60)
					{
						if (MeteorEffect == MeteorsEffect.Explode && ph > 0 && (colorX == (BufferWi / 2) + (int)(Math.Round(meteor.TailX * ph)) || colorX == (BufferWi / 2) - (int)(Math.Round(meteor.TailX * ph)) || colorY == (BufferHt / 2) + (int)(Math.Round(meteor.TailY * ph)) || colorY == (BufferHt / 2) - (int)(Math.Round(meteor.TailY * ph))))
						{
							break;
						}
						frameBuffer.SetPixel(colorX - (int)(Math.Round(meteor.TailX * ph)), colorY - (int)(Math.Round(meteor.TailY * ph)),
							hsv);
					}
				}
				if (colorX >= BufferWi + tailLength || colorY >= BufferHt + tailLength || colorX < 0 - tailLength ||
				    colorY < 0 - tailLength)
				{
					meteor.Expired = true; //flags Meteors that have reached the end of the grid as expiried.
					//	break;
				}
			}

			// delete old meteors
			int meteorNum = 0;
			while (meteorNum < _meteors.Count)
			{
				if (_meteors[meteorNum].Expired)
				{
					_meteors.RemoveAt(meteorNum);
				}
				else
				{
					meteorNum++;
				}
			}
		}

		private int CalculateSpeedVariation(double intervalPos)
		{
			var value = (int)ScaleCurveToValue(SpeedVariationCurve.GetValue(intervalPos), 200, 1);
			if (value < 1) value = 1;

			return value;
		}

		private int CalculateCenterSpeed(double intervalPos)
		{
			var value = (int)ScaleCurveToValue(CenterSpeedCurve.GetValue(intervalPos), 200, 1);
			if (value < 1) value = 1;

			return value;
		}

		private int CalculatePixelCount(double intervalPos)
		{
			var value = (int)ScaleCurveToValue(PixelCountCurve.GetValue(intervalPos), 200, 1);
			if (value < 1) value = 1;

			return value;
		}

		private int CalculateLength(double intervalPos)
		{
			var value = (int)ScaleCurveToValue(LengthCurve.GetValue(intervalPos), 100, 1);
			if (value < 1) value = 1;

			return value;
		}


		// for Meteor effects
		public class MeteorClass
		{
			public int X;
			public int Y;
			public double DeltaX;
			public double DeltaY;
			public double DeltaXOrig;
			public double DeltaYOrig;
			public double TailX;
			public double TailY;
			public HSV Hsv = new HSV();
			public bool Expired = false;
			public int Color;
			public double HsvBrightness;
		}

		// generates a random number between Color num1 and and Color num2.
		private static float RandomRange(float num1, float num2)
		{
			double hi, lo;
			InitRandom();

			if (num1 < num2)
			{
				lo = num1;
				hi = num2;
			}
			else
			{
				lo = num2;
				hi = num1;
			}
			return (float)(_random.NextDouble() * (hi - lo) + lo);
		}

		private int rand()
		{
			return _random.Next();
		}

		private static void InitRandom()
		{
			if (_random == null)
				_random = new Random();
		}

		//Use for Range type
		public static HSV SetRangeColor(HSV hsv1, HSV hsv2)
		{
			HSV newHsv = new HSV(RandomRange((float)hsv1.H, (float)hsv2.H),
								 RandomRange((float)hsv1.S, (float)hsv2.S),
								 1.0f);
			return newHsv;
		}
	}
}
