using PX.Data;

namespace PX.Objects.GL.Reclassification.Common
{
	public abstract class ReclassifyTransactionsBase<TGraph> : PXGraph<TGraph> 
		where TGraph : PXGraph
	{
		public PXFilter<ReclassGraphState> StateView;
		public PXSetup<GLSetup> GLSetup;

		protected ReclassGraphState State
		{
			get { return StateView.Current; }
			set { StateView.Current = value; }
		}

		protected ReclassifyTransactionsBase()
		{
			var setup = GLSetup.Current;
		}
	}
}
