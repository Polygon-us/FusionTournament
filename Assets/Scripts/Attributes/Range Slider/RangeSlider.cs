namespace Attributes
{ 
	public class RangeSlider : Name
	{
		#region Information

		public float min;
		public float max;

		#endregion

		public RangeSlider(float min, float max, string name = null) : base(name)
		{
			this.min = min;
			this.max = max;
		}
	}
}
