﻿using System;
using PX.Common;
using PX.Data;


namespace PX.Objects.RUTROT
{
	public interface IRUTROTable
	{
		bool? IsRUTROTDeductible { get; set; }
		string GetDocumentType();
		string GetDocumentNbr();
		bool? GetRUTROTCompleted();
		int? GetDocumentBranchID();
		string GetDocumentCuryID();
		bool? GetDocumentHold();
		IBqlTable GetBaseDocument();
	}
	public interface IRUTROTableLine
	{
		int? GetInventoryID();
		bool? IsRUTROTDeductible { get; set; }
		String RUTROTItemType { get; set; }
		Int32? RUTROTWorkTypeID { get; set; }
		decimal? CuryRUTROTTaxAmountDeductible { get; set; }
		decimal? RUTROTTaxAmountDeductible { get; set; }
		decimal? CuryRUTROTAvailableAmt { get; set; }
		decimal? RUTROTAvailableAmt { get; set; }
	}
}
