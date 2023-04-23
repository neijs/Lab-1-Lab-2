#include "pch.h"
#include "mkl.h"
#include "mkl_df_defines.h"

extern "C" __declspec(dllexport) void MKLSplines(MKL_INT nodeQnt, double boundA, double boundB, bool uniform,
    const double* nodes, const double* values, MKL_INT nGrid, double derivLeft, double derivRight, double* mes, 
	double* der1, double* der2, double& integValue, int& respCode);

void MKLSplines(MKL_INT nodeQnt, double boundA, double boundB, bool uniform,
    const double* nodes, const double* values, MKL_INT nGrid, double derivLeft, double derivRight, double* mes,
	double* der1, double* der2, double& integValue, int& respCode)
{
	try 
	{
		DFTaskPtr task;

		// FIELD HAS 1 MEASURMENTS
		CONST MKL_INT vectDim = 1;

		// DORDER's NUMBER OF UNITS (ones, '1')
		CONST MKL_INT nder = 3;

		// UNIFORM BOUNDARIES
		double* uniBounds = new double[2];
		uniBounds[0] = boundA;
		uniBounds[1] = boundB;

		// DERIVATIVE ON EDGE
		double* deriv = new double[2];
		deriv[0] = derivLeft;
		deriv[1] = derivRight;

		// KOSTIIL FOR INTEGRALS
		double* leftEnd = new double[1];
		leftEnd[0] = boundA;
		double* rightEnd = new double[1];
		rightEnd[0] = boundB;
		double* integ = new double[1];

		// QUANTITY OF INTEGRALS TO CALCULATE
		MKL_INT integQnty = 1;

		// DERIVATIVE ORDER TO CALCULATE
		MKL_INT ndorder = 3;

		// SPLINE AND FIRST/SECOND SPLINE DERIVATIVE
		MKL_INT* dorder = new MKL_INT[ndorder] { 1, 1, 1 };

		// ARRAY OF SPLINE COEFFICIENTS VALUES
		double* splineCoeff = new double[vectDim * DF_PP_CUBIC * (nodeQnt - 1)];

		// THE RESULT: MES1, DER1, DER2
		double* result = new double[vectDim * nder * nGrid];

		/*-----------------------------------------------------------------------------------------------------*/
		// CREATE TASK
		if (uniform == TRUE) {
			respCode = dfdNewTask1D(&task, nodeQnt, uniBounds, DF_UNIFORM_PARTITION, vectDim, values, DF_MATRIX_STORAGE_ROWS);
		}
		else {
			respCode = dfdNewTask1D(&task, nodeQnt, nodes, DF_NON_UNIFORM_PARTITION, vectDim, values, DF_MATRIX_STORAGE_ROWS);
		}
		if (respCode != DF_STATUS_OK) return;

		// EDIT SPLINE CONFIGURATION
		respCode = dfdEditPPSpline1D(task, DF_PP_CUBIC, DF_PP_NATURAL, (DF_BC_1ST_LEFT_DER | DF_BC_1ST_RIGHT_DER),
			deriv, DF_NO_IC, NULL, splineCoeff, DF_NO_HINT);
		if (respCode != DF_STATUS_OK) return;

		// CONSTRUCT SPLINE
		respCode = dfdConstruct1D(task, DF_PP_SPLINE, DF_METHOD_STD);
		if (respCode != DF_STATUS_OK) return;

		// CALCULATE SPLINE
		respCode = dfdInterpolate1D(task, DF_INTERP, DF_METHOD_PP, nGrid, uniBounds, DF_UNIFORM_PARTITION,
			ndorder, dorder, NULL, result, DF_MATRIX_STORAGE_ROWS, NULL);
		if (respCode != DF_STATUS_OK) return;

		// MES1, MES2, DER1, DER2 EXTRACTION
		for (int i = 0, j = 0, k = 0, m = 0; i < nder * nGrid * vectDim; ++i) {
			if (i % 3 == 0) {
				mes[j] = result[i];
				++j;
			}
			else if (i % 3 == 1) {
				der1[k] = result[i];
				++k;
			}
			else if (i % 3 == 2) {
				der2[m] = result[i];
				++m;
			}
		} // mes1[0] der1[0] mes1[1] der[1] ... mes2[0] der2[0] mes2[1] der2[1] ...
		// CALCULATE INTEGRALS
		respCode = dfdIntegrate1D(task, DF_METHOD_PP, integQnty, leftEnd, DF_NO_HINT,
			rightEnd, DF_NO_HINT, NULL, NULL, integ, DF_MATRIX_STORAGE_ROWS);
		if (respCode != DF_STATUS_OK) return;
		integValue = integ[0];

		// DELETE TASK
		respCode = dfDeleteTask(&task);
		if (respCode != DF_STATUS_OK) return;

		/*-----------------------------------------------------------------------------------------------------*/

		respCode = 0;

		delete[] uniBounds;
		delete[] deriv;
		delete[] splineCoeff;
		delete[] leftEnd;
		delete[] rightEnd;
		delete[] integ;
		delete[] result;
	}
	catch (...)
	{
		respCode = -1;
	}
}
    