using System;
using PX.Data;
using System.Collections;
using System.Collections.Generic;
using PX.Objects.CM;

namespace PX.Objects.GL
{
	//[PXGraphName(Messages.GLNumberCodesMaint, typeof(GLNumberCode))]
	public class GLNumberCodeMaint : PXGraph<GLNumberCodeMaint, GLNumberCode>
	{        
		[PXImport(typeof(GLNumberCode))]
		public PXSelect<GLNumberCode> NumberCodes;
	}
}
