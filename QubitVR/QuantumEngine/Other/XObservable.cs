using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Complex = System.Numerics.Complex;
using MathNet.Numerics.LinearAlgebra;

/// <summary>
/// Quantum observable for the expectation value along the X axis.
/// </summary>
public class XObservable : Observable
{
    /// <summary>
    /// The qubit index in which the XObservable will be calculated for.
    /// </summary>
    public int m_qubitIndex { get; }

    /// <summary>
    /// Constructs an XObservable given the number of qubits and a qubit index.
    /// </summary>
    /// <param name="qubits">Number of qubits in the system.</param>
    /// <param name="xIndex">Qubit index to create the observable from.</param>
    public XObservable(int qubits, int xIndex)
    {
        // Ensure positive integer number of qubits
        if (qubits < 1)
        {
            Debug.LogError("Number of qubits must be a positive integer.");
            return;
        }

        // Check that the xIndex is appropriate for the number of qubits in the system.
        if (xIndex < 0 || xIndex > qubits - 1)
        {
            Debug.LogError("Index provided to constructor XObservable(int qubits, int xIndex) is out of range. Index of " + xIndex +
                " provided. Index in range [0, " + (qubits - 1) + "] expected.");
            return;
        }

        // Set XObservable local variables
        m_numQubits = qubits;
        m_qubitIndex = xIndex;
        m_type = "XObservable";

        // Generate matrix for the X Observable
        Matrix<Complex> identity = Matrix<Complex>.Build.SparseIdentity(2, 2);
        Matrix<Complex> pauliX = Matrix<Complex>.Build.Sparse(2, 2);
        pauliX[0, 1] = new Complex(1, 0);
        pauliX[1, 0] = new Complex(1, 0);

        // Set the initial value of m_matrix based on xIndex. Initial value is
        // the X Observable if the first index is the xIndex, identity otherwise.
        if (xIndex == 0)
            m_matrix = pauliX;
        else
            m_matrix = identity;

        // For each additional qubit, kronecker product the current state with the X Observable if that qubits index
        // is the xIndex; kronecker product with the identity matrix otherwise.
        for (int currentIndex = 1; currentIndex < m_numQubits; currentIndex++)
        {
            if (currentIndex == xIndex)
                m_matrix = m_matrix.KroneckerProduct(pauliX);
            else
                m_matrix = m_matrix.KroneckerProduct(identity);
        }
    }
}