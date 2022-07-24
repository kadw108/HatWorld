/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RWCustom;
using UnityEngine;

namespace HatWorld
{
    public class ShapeCustom : NSHSwarmer.Shape
    {
		public ShapeCustom(NSHSwarmer.Shape owner, NSHSwarmer.Shape.ShapeType shapeType, Vector3 pos, float width, float height) : base(owner, shapeType, pos, width, height)
		{
			this.owner = owner;
			this.shapeType = shapeType;
			this.pos = pos;
			this.startPos = pos;
			this.lastPos = pos;

			switch (shapeType)
			{
				case NSHSwarmer.Shape.ShapeType.Main:
					this.subShapes.Add(new NSHSwarmer.Shape(this, NSHSwarmer.Shape.ShapeType.Shell, pos, 9f, 22f));
					this.subShapes.Add(new NSHSwarmer.Shape(this, NSHSwarmer.Shape.ShapeType.Belt, pos, 25f, 6f));
					// this.subShapes.Add(new NSHSwarmer.Shape(this, NSHSwarmer.Shape.ShapeType.DiamondHolder, pos, 35f, 0f));
					// this.subShapes.Add(new NSHSwarmer.Shape(this, NSHSwarmer.Shape.ShapeType.Cube, pos, 27f, 27f));
					// this.subShapes.Add(new NSHSwarmer.Shape(this, NSHSwarmer.Shape.ShapeType.Ribbon, pos, 44f, 7f));
					// this.subShapes.Add(new NSHSwarmer.Shape(this, NSHSwarmer.Shape.ShapeType.Sphere, pos, 36f, 28f));
					// this.subShapes.Add(new NSHSwarmer.Shape(this, NSHSwarmer.Shape.ShapeType.BigDiamonds, pos, 39f, 11f));
					break;
				case NSHSwarmer.Shape.ShapeType.Shell:
					{
						int num = 5;
						this.verts.Add(new NSHSwarmer.Shape.Vert(0f, 0f, -height));
						this.verts[0].B *= 0.5f;
						this.verts[0].C *= 1.5f;
						this.verts.Add(new NSHSwarmer.Shape.Vert(0f, 0f, height));
						this.verts[1].B *= 0.5f;
						this.verts[1].C *= 1.5f;
						for (int i = 0; i < num; i++)
						{
							float num2 = (float)i / (float)num;
							Vector2 vector = Custom.DegToVec(num2 * 360f) * width;
							this.verts.Add(new NSHSwarmer.Shape.Vert(vector.x, vector.y, -height / 2.2f));
							this.verts[this.verts.Count - 1].B *= 1.2f;
							NSHSwarmer.Shape.Vert vert = this.verts[this.verts.Count - 1];
							vert.B.z = vert.B.z * 0.2f;
							this.verts[this.verts.Count - 1].C *= 0.8f;
							NSHSwarmer.Shape.Vert vert2 = this.verts[this.verts.Count - 1];
							vert2.C.z = vert2.C.z * 2f;
							this.verts.Add(new NSHSwarmer.Shape.Vert(vector.x, vector.y, height / 2.2f));
							this.verts[this.verts.Count - 1].B *= 1.2f;
							NSHSwarmer.Shape.Vert vert3 = this.verts[this.verts.Count - 1];
							vert3.B.z = vert3.B.z * 0.2f;
							this.verts[this.verts.Count - 1].C *= 0.8f;
							NSHSwarmer.Shape.Vert vert4 = this.verts[this.verts.Count - 1];
							vert4.C.z = vert4.C.z * 2f;
						}
						for (int j = 0; j < num; j++)
						{
							int num3 = (j >= num - 1) ? 0 : (j + 1);
							this.holoLines.Add(new NSHSwarmer.Shape.Line(this.verts[2 + j * 2], this.verts[0]));
							this.holoLines.Add(new NSHSwarmer.Shape.Line(this.verts[2 + j * 2], this.verts[2 + num3 * 2]));
							this.holoLines.Add(new NSHSwarmer.Shape.Line(this.verts[2 + j * 2 + 1], this.verts[2 + num3 * 2 + 1]));
							this.holoLines.Add(new NSHSwarmer.Shape.Line(this.verts[2 + j * 2], this.verts[2 + j * 2 + 1]));
							this.holoLines.Add(new NSHSwarmer.Shape.Line(this.verts[2 + j * 2 + 1], this.verts[1]));
						}
						break;
					}
				case NSHSwarmer.Shape.ShapeType.Belt:
					{
						int num = 7;
						for (int k = 0; k < num; k++)
						{
							float num4 = (float)k / (float)num;
							Vector2 vector2 = Custom.DegToVec(num4 * 360f);
							this.verts.Add(new NSHSwarmer.Shape.Vert(vector2.x * width, vector2.y * width, -height * 1.2f));
							this.verts[this.verts.Count - 1].B = new Vector3(vector2.x * (width - height * 0.25f), vector2.y * (width - height * 0.25f), 0f);
							this.verts[this.verts.Count - 1].C = new Vector3(vector2.x * (width + height * 2f), vector2.y * (width + height * 1.5f), 0f);
							this.verts.Add(new NSHSwarmer.Shape.Vert(vector2.x * width, vector2.y * width, height * 1.2f));
							this.verts[this.verts.Count - 1].B = new Vector3(vector2.x * (width + height * 2f), vector2.y * (width + height * 2f), 0f);
							this.verts[this.verts.Count - 1].C = new Vector3(vector2.x * (width - height * 0.25f), vector2.y * (width - height * 0.25f), 0f);
						}
						for (int l = 0; l < num; l++)
						{
							int num5 = (l >= num - 1) ? 0 : (l + 1);
							this.holoLines.Add(new NSHSwarmer.Shape.Line(this.verts[l * 2], this.verts[num5 * 2]));
							this.holoLines.Add(new NSHSwarmer.Shape.Line(this.verts[l * 2 + 1], this.verts[num5 * 2 + 1]));
							this.holoLines.Add(new NSHSwarmer.Shape.Line(this.verts[l * 2], this.verts[l * 2 + 1]));
						}
						break;
					}
				case NSHSwarmer.Shape.ShapeType.DiamondHolder:
					{
						int num = 5;
						for (int m = 0; m < num; m++)
						{
							float num6 = (float)m / (float)num;
							Vector2 vector3 = Custom.DegToVec(num6 * 360f) * width;
							this.subShapes.Add(new NSHSwarmer.Shape(this, NSHSwarmer.Shape.ShapeType.Diamond, new Vector3(vector3.x, vector3.y, 0f), 3f, 5f));
						}
						this.subShapes.Add(new NSHSwarmer.Shape(this, NSHSwarmer.Shape.ShapeType.SmallDiamondHolder, pos + new Vector3(0f, 0f, -22f), 20f, 0f));
						this.subShapes.Add(new NSHSwarmer.Shape(this, NSHSwarmer.Shape.ShapeType.SmallDiamondHolder, pos + new Vector3(0f, 0f, 22f), 20f, 0f));
						break;
					}
				case NSHSwarmer.Shape.ShapeType.SmallDiamondHolder:
					{
						int num = 3;
						for (int n = 0; n < num; n++)
						{
							float num7 = (float)n / (float)num;
							Vector2 vector4 = Custom.DegToVec(num7 * 360f) * width;
							this.subShapes.Add(new NSHSwarmer.Shape(this, NSHSwarmer.Shape.ShapeType.Diamond, new Vector3(vector4.x, vector4.y, 0f), 3f, 5f));
						}
						break;
					}
				case NSHSwarmer.Shape.ShapeType.Diamond:
					{
						this.verts.Add(new NSHSwarmer.Shape.Vert(-width, 0f, 0f));
						this.verts.Add(new NSHSwarmer.Shape.Vert(0f, 0f, height));
						this.verts.Add(new NSHSwarmer.Shape.Vert(width, 0f, 0f));
						this.verts.Add(new NSHSwarmer.Shape.Vert(0f, 0f, -height));
						int num = 4;
						for (int num8 = 0; num8 < num; num8++)
						{
							int index = (num8 >= num - 1) ? 0 : (num8 + 1);
							this.holoLines.Add(new NSHSwarmer.Shape.Line(this.verts[num8], this.verts[index]));
						}
						break;
					}
				case NSHSwarmer.Shape.ShapeType.Cube:
					{
						int num = 4;
						for (int num9 = 0; num9 < num; num9++)
						{
							float num10 = (float)num9 / (float)num;
							Vector2 vector5 = Custom.DegToVec(num10 * 360f) * width * 1.42f;
							this.verts.Add(new NSHSwarmer.Shape.Vert(vector5.x, vector5.y, -height));
							this.verts[this.verts.Count - 1].B = this.MultVec(this.verts[this.verts.Count - 1].B, new Vector3(1.4f, 1.4f, 0.2f));
							this.verts[this.verts.Count - 1].C *= 0.2f;
							this.verts.Add(new NSHSwarmer.Shape.Vert(vector5.x, vector5.y, height));
							this.verts[this.verts.Count - 1].B = this.MultVec(this.verts[this.verts.Count - 1].B, new Vector3(1.4f, 1.4f, 0.2f));
							this.verts[this.verts.Count - 1].C *= 0.2f;
						}
						for (int num11 = 0; num11 < num; num11++)
						{
							int num12 = (num11 >= num - 1) ? 0 : (num11 + 1);
							this.holoLines.Add(new NSHSwarmer.Shape.Line(this.verts[num11 * 2], this.verts[num12 * 2]));
							this.holoLines.Add(new NSHSwarmer.Shape.Line(this.verts[num11 * 2 + 1], this.verts[num12 * 2 + 1]));
							this.holoLines.Add(new NSHSwarmer.Shape.Line(this.verts[num11 * 2], this.verts[num11 * 2 + 1]));
						}
						break;
					}
				case NSHSwarmer.Shape.ShapeType.Ribbon:
					{
						int num = 22;
						for (int num13 = 0; num13 < num; num13++)
						{
							float num14 = (float)num13 / (float)num;
							Vector2 vector6 = Custom.DegToVec(num14 * 360f) * width;
							this.verts.Add(new NSHSwarmer.Shape.Vert(vector6.x, vector6.y, -height));
							this.verts.Add(new NSHSwarmer.Shape.Vert(vector6.x, vector6.y, height));
							vector6 = Custom.DegToVec(((float)num13 + ((num13 % 2 != 0) ? 0.5f : -0.5f)) / (float)num * 360f) * width;
							this.verts[this.verts.Count - 2].B = new Vector3(vector6.x, vector6.y, -height * 0.75f);
							this.verts[this.verts.Count - 1].B = new Vector3(vector6.x, vector6.y, height * 0.75f);
							vector6 = Custom.DegToVec(((float)num13 + ((num13 % 2 != 0) ? -0.5f : 0.5f)) / (float)num * 360f) * width;
							this.verts[this.verts.Count - 2].C = new Vector3(vector6.x, vector6.y, -height * 1.5f);
							this.verts[this.verts.Count - 1].C = new Vector3(vector6.x, vector6.y, height * 1.5f);
						}
						for (int num15 = 0; num15 < num; num15++)
						{
							int num16 = (num15 >= num - 1) ? 0 : (num15 + 1);
							if (num15 % 2 == 0)
							{
								this.holoLines.Add(new NSHSwarmer.Shape.Line(this.verts[num15 * 2], this.verts[num16 * 2]));
								this.holoLines.Add(new NSHSwarmer.Shape.Line(this.verts[num15 * 2 + 1], this.verts[num16 * 2 + 1]));
							}
							this.holoLines.Add(new NSHSwarmer.Shape.Line(this.verts[num15 * 2], this.verts[num15 * 2 + 1]));
						}
						break;
					}
				case NSHSwarmer.Shape.ShapeType.Sphere:
					{
						int num = 18;
						for (int num17 = 0; num17 < 2; num17++)
						{
							for (int num18 = 0; num18 < num; num18++)
							{
								float num19 = (float)num18 / (float)num;
								Vector2 vector7 = Custom.DegToVec(num19 * 360f) * width;
								this.verts.Add(new NSHSwarmer.Shape.Vert(vector7.x, vector7.y, height * 0.5f * (float)((num17 != 0) ? 1 : -1)));
								this.verts.Add(new NSHSwarmer.Shape.Vert(vector7.x * 0.72f, vector7.y * 0.75f, height * (float)((num17 != 0) ? 1 : -1)));
								vector7 = Custom.DegToVec(((float)num18 + ((num18 % 2 != num17) ? 0.5f : -0.5f)) / (float)num * 360f) * width;
								this.verts[this.verts.Count - 2].B = new Vector3(vector7.x, vector7.y, height * 0.5f * (float)((num17 != 0) ? 1 : -1));
								this.verts[this.verts.Count - 1].B = new Vector3(vector7.x * 0.72f, vector7.y * 0.72f, height * (float)((num17 != 0) ? 1 : -1));
								vector7 = Custom.DegToVec(((float)num18 + ((num18 % 2 == num17) ? 0.5f : -0.5f)) / (float)num * 360f) * width;
								this.verts[this.verts.Count - 2].C = new Vector3(vector7.x, vector7.y, height * 0.5f * (float)((num17 != 0) ? 1 : -1));
								this.verts[this.verts.Count - 1].C = new Vector3(vector7.x * 0.72f, vector7.y * 0.72f, height * (float)((num17 != 0) ? 1 : -1));
							}
						}
						for (int num20 = 0; num20 < 2; num20++)
						{
							for (int num21 = 0; num21 < num; num21++)
							{
								int num22 = (num21 >= num - 1) ? 0 : (num21 + 1);
								if (num21 % 2 == num20)
								{
									this.holoLines.Add(new NSHSwarmer.Shape.Line(this.verts[num * 2 * num20 + num21 * 2], this.verts[num * 2 * num20 + num22 * 2]));
									this.holoLines.Add(new NSHSwarmer.Shape.Line(this.verts[num * 2 * num20 + num21 * 2 + 1], this.verts[num * 2 * num20 + num22 * 2 + 1]));
								}
								this.holoLines.Add(new NSHSwarmer.Shape.Line(this.verts[num * 2 * num20 + num21 * 2], this.verts[num * 2 * num20 + num21 * 2 + 1]));
							}
						}
						break;
					}
				case NSHSwarmer.Shape.ShapeType.BigDiamonds:
					{
						int num = 7;
						for (int num23 = 0; num23 < num; num23++)
						{
							Vector2 vector8 = Custom.DegToVec((float)num23 / (float)num * 360f) * (width - height / 3f);
							Vector2 vector9 = Custom.DegToVec(((float)num23 - 0.15f) / (float)num * 360f) * width;
							Vector2 vector10 = Custom.DegToVec(((float)num23 + 0.15f) / (float)num * 360f) * width;
							this.verts.Add(new NSHSwarmer.Shape.Vert(vector8.x, vector8.y, -height));
							this.verts.Add(new NSHSwarmer.Shape.Vert(vector9.x, vector9.y, 0f));
							this.verts.Add(new NSHSwarmer.Shape.Vert(vector8.x, vector8.y, height));
							this.verts.Add(new NSHSwarmer.Shape.Vert(vector10.x, vector10.y, 0f));
							for (int num24 = 0; num24 < 4; num24++)
							{
								int num25 = (num24 >= 3) ? 0 : (num24 + 1);
								this.holoLines.Add(new NSHSwarmer.Shape.Line(this.verts[num23 * 4 + num24], this.verts[num23 * 4 + num25]));
							}
						}
						this.subShapes.Add(new NSHSwarmer.Shape(this, NSHSwarmer.Shape.ShapeType.BigDiamonds2, pos, width, height - 3.5f));
						break;
					}
				case NSHSwarmer.Shape.ShapeType.BigDiamonds2:
					{
						int num = 7;
						for (int num26 = 0; num26 < num; num26++)
						{
							Vector2 vector11 = Custom.DegToVec((float)num26 / (float)num * 360f) * (width - height / 5f);
							Vector2 vector12 = Custom.DegToVec(((float)num26 - 0.08f) / (float)num * 360f) * width;
							Vector2 vector13 = Custom.DegToVec(((float)num26 + 0.08f) / (float)num * 360f) * width;
							this.verts.Add(new NSHSwarmer.Shape.Vert(vector11.x, vector11.y, -height));
							this.verts.Add(new NSHSwarmer.Shape.Vert(vector12.x, vector12.y, 0f));
							this.verts.Add(new NSHSwarmer.Shape.Vert(vector11.x, vector11.y, height));
							this.verts.Add(new NSHSwarmer.Shape.Vert(vector13.x, vector13.y, 0f));
							for (int num27 = 0; num27 < 4; num27++)
							{
								int num28 = (num27 >= 3) ? 0 : (num27 + 1);
								this.holoLines.Add(new NSHSwarmer.Shape.Line(this.verts[num26 * 4 + num27], this.verts[num26 * 4 + num28]));
							}
						}
						break;
					}
			}
		}
	}
}
*/
