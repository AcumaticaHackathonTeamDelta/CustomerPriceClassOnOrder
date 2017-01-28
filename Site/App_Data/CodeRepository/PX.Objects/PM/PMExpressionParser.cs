using System;
using PX.Common.Parser;
using PX.Reports.Parser;

namespace PX.Objects.PM
{	
    public class PMExpressionParser : ExpressionParser
	{
		public PMAllocator Engine { get; protected set; }
		public PMAllocationStep Step { get; protected set; }

		private PMExpressionParser(PMAllocator engine, PMAllocationStep step, string text)
			: base(text)
		{
			Engine = engine;
			Step = step;
		}

		protected override ExpressionContext CreateContext()
		{
			return new PMExpressionContext(Engine, Step);
		}

		protected override NameNode CreateNameNode(ExpressionNode node, string tokenString)
		{
			return new PMNameNode(node, tokenString, Context);

		}

		protected override void ValidateName(NameNode node, string tokenString)
		{
			
		}
		
		protected override bool IsAggregate(string nodeName)
		{
			return ReportAggregateNode.IsAggregate(nodeName);
		}

		protected override AggregateNode CreateAggregateNode(string name, string dataField)
		{
			return new ReportAggregateNode(name, dataField);
		}

		public static ExpressionNode Parse(PMAllocator engine, PMAllocationStep step, string formula)
		{
			if (formula.StartsWith("="))
			{
				formula = formula.Substring(1);
			}

			var expr = new PMExpressionParser(engine, step, formula);
			return expr.Parse();
		}
	}

	public class PMExpressionContext : ExpressionContext
	{
		protected PMAllocator engine;
		protected PMAllocationStep step;

		public PMExpressionContext(PMAllocator engine, PMAllocationStep step)
		{
			this.engine = engine;
			this.step = step;
		}
				
		public virtual object Evaluate(PMNameNode node, PMTran row)
		{
			if ( node.IsAttribute )
				return engine.Evaluate(node.ObjectName, null, node.FieldName, row);
			else
				return engine.Evaluate(node.ObjectName, node.FieldName, null, row);
		}
	}

	public class PMNameNode : NameNode
	{
		public PMObjectType ObjectName { get; protected set; }
		public string FieldName { get; protected set; }
		protected bool isAttribute;
		
		public PMNameNode(ExpressionNode node, string tokenString, ExpressionContext context)
			: base(node, tokenString, context)
		{
			string[] parts = Name.Split('.');

			if (parts.Length == 3)
			{
				isAttribute = true;
				ObjectName = (PMObjectType)Enum.Parse(typeof(PMObjectType), parts[0], true);
				FieldName = parts[2].Trim('[',']').Trim();
			}
			else if (parts.Length == 2)
			{
				ObjectName = (PMObjectType)Enum.Parse(typeof(PMObjectType), parts[0], true);
				if (parts[1].Trim().EndsWith("_Attributes"))
				{
					isAttribute = true;
					FieldName = parts[1].Substring(0, parts[1].Length - 11);
				}
				else
					FieldName = parts[1];
			}
			else
			{
				ObjectName = PMObjectType.PMTran;
				FieldName = Name;
			}
		}

		protected bool IsParameter
		{
			get
			{
				return name.StartsWith("@");
			}
		}

		public bool IsAttribute
		{
			get { return isAttribute; }
		}

		public override object Eval(object row)
		{
			if (IsParameter)
			{
				return ((PMTran)row).Rate;
			}

			return ((PMExpressionContext)context).Evaluate(this, (PMTran)row);
		}

	}

	public enum PMObjectType
	{
		PMTran,
		PMProject,
		PMTask,
		PMAccountGroup,
		EPEmployee,
		Customer,
		Vendor,
		InventoryItem
	}
}
