using Godot;

namespace Rhythmer
{
	public struct PlaybackMarker
	{
		public double Time;
		public AudioStream Stream;
		public string IdType;
		public int Id;

		public PlaybackMarker(double Time, AudioStream Stream, string IdType = "", int Id = 0)
		{
			this.Time = Time;
			this.Stream = Stream;
			this.IdType = IdType;
			this.Id = Id;
		}
	}
}
