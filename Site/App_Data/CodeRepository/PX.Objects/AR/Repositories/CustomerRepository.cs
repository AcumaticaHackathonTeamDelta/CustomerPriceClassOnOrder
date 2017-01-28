using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Common;
using PX.Data;
using PX.Objects.CR;
using PX.SM;

namespace PX.Objects.AR.Repositories
{
	public class CustomerRepository
	{
		protected readonly PXGraph _graph;

		public CustomerRepository(PXGraph graph)
		{
			if (graph == null) throw new ArgumentNullException(nameof(graph));

			_graph = graph;
		}

		public Customer FindByID(int? accountID)
		{
			foreach (PXResult<BAccount, Customer> result in PXSelectJoin<
				BAccount,
					InnerJoinSingleTable<Customer,
						On<BAccount.bAccountID, Equal<Customer.bAccountID>>>,
				Where2<
					Where<
						BAccount.type, Equal<BAccountType.customerType>,
						Or<BAccount.type, Equal<BAccountType.combinedType>>>,
					And<Customer.bAccountID, Equal<Required<Customer.bAccountID>>>>>
				.Select(_graph, accountID))
			{
				BAccount businessAccount = result;
				Customer customer = result;

				PXCache<BAccount>.RestoreCopy(customer, businessAccount);

				return customer;
			}

			return null;
		}

		public Customer FindByCD(string accountCD)
		{
			foreach (PXResult<BAccount, Customer> result in PXSelectJoin<
				BAccount,
					InnerJoinSingleTable<Customer,
						On<BAccount.bAccountID, Equal<Customer.bAccountID>>>,
				Where2<
					Where<
						BAccount.type, Equal<BAccountType.customerType>,
						Or<BAccount.type, Equal<BAccountType.combinedType>>>,
					And<BAccount.acctCD, Equal<Required<Customer.acctCD>>>>>
				.Select(_graph, accountCD))
			{
				BAccount businessAccount = result;
				Customer customer = result;

				PXCache<BAccount>.RestoreCopy(customer, businessAccount);

				return customer;
			}

			return null;
		}

		public Customer GetByCD(string accountCD)
		{
			var customer = FindByCD(accountCD);

			if (customer == null)
			{
				throw new PXSetPropertyException(PXMessages.LocalizeFormat(ErrorMessages.ValueDoesntExist,
					Messages.Customer, accountCD));
			}

			return customer;
		}
	}
}
