using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Complex = System.Numerics.Complex;
using MathNet.Numerics.LinearAlgebra;

/// <summary>
/// Parent class for quantum projectors. Useful for quantum measurement.
/// </summary>
public class Projector
{
    /// <summary>
    /// Number of qubits in the system.
    /// </summary>
    protected int m_numQubits;
    /// <summary>
    /// The matrix for the system. rows = 2^m_numQubits; cols = 1.
    /// </summary>
    protected Matrix<Complex> m_matrix;
    /// <summary>
    /// String that describes the type of projector in use.
    /// </summary>
    protected string m_type;

    /// <summary>
    /// Getter for protected field 'm_matrix'.
    /// </summary>
    /// <returns>The complex matrix of the current projector.</returns>
    public Matrix<Complex> GetMatrix() { return m_matrix; }
}