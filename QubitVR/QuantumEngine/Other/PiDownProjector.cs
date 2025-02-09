using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Complex = System.Numerics.Complex;
using MathNet.Numerics.LinearAlgebra;

/// <summary>
/// Quantum projector representing the down basis.
/// </summary>
public class PiDownProjector : Projector
{
    /// <summary>
    /// The qubit index in which the PiDownProjector will be calculated for, and thus projected on to.
    /// </summary>
    public int m_qubitIndex { get; }

    /// <summary>
    /// Constructs a PiDownProjector given the number of qubits and a qubit index to project on to.
    /// </summary>
    /// <param name="qubits">Number of qubits in the system.</param>
    /// <param name="downIndex">Qubit index to apply the down projector to.</param>
    public PiDownProjector(int qubits, int downIndex)
    {
        // Ensure positive integer number of qubits
        if (qubits < 1)
        {
            Debug.LogError("Number of qubits must be a positive integer.");
            return;
        }

        // Check that the downIndex is appropriate for the number of qubits in the system.
        if (downIndex < 0 || downIndex > qubits - 1)
        {
            Debug.LogError("Index provided to constructor PiDownProjector(int qubits, int downIndex) is out of range. Index of " + downIndex +
                " provided. Index in range [0, " + (qubits - 1) + "] expected.");
            return;
        }

        // Set PiDownProjector local variables
        m_numQubits = qubits;
        m_qubitIndex = downIndex;
        m_type = "PiDown";

        // Generate matrix for the Pi-Down projector
        Matrix<Complex> identity = Matrix<Complex>.Build.SparseIdentity(2, 2);
        Matrix<Complex> piDown = Matrix<Complex>.Build.Sparse(2, 2);
        piDown[1, 1] = new Complex(1, 0);

        // Set the initial value of m_matrix based on downIndex. Initial value is
        // the Pi-Down projector if the first index is the downIndex, identity otherwise.
        if (downIndex == 0)
            m_matrix = piDown;
        else
            m_matrix = identity;

        // For each additional qubit, kronecker product the current state with the Pi-Down projector if that qubits index
        // is the downIndex; kronecker product with the identity matrix otherwise.
        for (int currentIndex = 1; currentIndex < m_numQubits; currentIndex++)
        {
            if (currentIndex == downIndex)
                m_matrix = m_matrix.KroneckerProduct(piDown);
            else
                m_matrix = m_matrix.KroneckerProduct(identity);
        }
    }
}
