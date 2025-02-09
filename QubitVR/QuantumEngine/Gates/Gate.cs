using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Complex = System.Numerics.Complex;
using MathNet.Numerics.LinearAlgebra;

/// <summary>
/// Parent class for quantum gates.
/// </summary>
public class Gate
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
    /// String that describes the type of gate in use.
    /// </summary>
    protected string m_type;

    /// <summary>
    /// Getter for protected field 'm_matrix'.
    /// </summary>
    /// <returns>The complex matrix of the current gate.</returns>
    public Matrix<Complex> GetMatrix() { return m_matrix; }
    new public string GetType() { return m_type; }
    public int GetNumQubits() { return m_numQubits; }

    /// <summary>
    /// Constructor used by NthRootGate to construct a general Gate object. It can also be used to generate a 'general' gate
    ///  with any valid matrix. This is not the intended use and is not recommended.
    /// </summary>
    /// <param name="qubits">Number of qubits in the system.</param>
    /// <param name="matrix">Matrix of the qubit system.</param>
    /// <param name="type">String specifying the type of gate.</param>
    public Gate(int qubits, Matrix<Complex> matrix, string type = "undefined")
    {
        // Ensure positive integer number of qubits
        if (qubits < 1)
        {
            Debug.LogError("Number of qubits must be a positive integer.");
            return;
        }

        // Ensure that the user didnt pass in a null matrix to break the program.
        if (matrix == null)
        {
            Debug.LogError("Null matrix provided");
            return;
        }

        // Ensure that the input matrix is square and has the correct number of rows/columns according to the number of qubits passed in.
        if (matrix.RowCount != matrix.ColumnCount || Math.Pow(2, qubits) != matrix.RowCount)
        {
            Debug.LogError("Matrix provided to Gate constructor is not the correct size. Either the matrix is not square or it is not built for the number" +
                "of qubits specified.");
            return;
        }

        m_numQubits = qubits;
        m_matrix = matrix;
        m_type = type;
    }

    /// <summary>
    /// Default constructor because C# is forcing me to. This isnt used and I dont understand why I need this.
    /// </summary>
    public Gate(int qubits = 1)
    {
        m_numQubits = qubits;
        int dimensions = (int)Math.Pow(2, qubits);
        m_matrix = Matrix<Complex>.Build.SparseIdentity(dimensions, dimensions);
        m_type = "undefined";
    }

    /// <summary>
    /// Construct an Nth root gate from the current gate.
    /// </summary>
    /// <param name="n">The root degree to take of the current gate matrix.
    /// n=2 -> square root of the current gate matrix. n=3 -> cube root of the current gate matrix. etc.</param>
    /// <returns>Completed NthRootGate object</returns>
    public virtual Gate NthRootGate(int n)
    {
        // Ensure that NthRoot to be calculated is valid. Must be a positive integer.
        if (n < 1)
        {
            Debug.LogError("Must provide a positive integer for NthRootGate");
            return new Gate(0, NthRootMatrix(1));
        }

        return new Gate(0, NthRootMatrix(n));
    }

    /// <summary>
    /// Construct the Nth root matrix of the current gate.
    /// </summary>
    /// <param name="n">The root degree to take of the current gate matrix.
    /// n=2 -> square root of the current gate matrix. n=3 -> cube root of the current gate matrix. etc.</param>
    /// <returns>Completed NthRootMatrix</returns>
    protected virtual Matrix<Complex> NthRootMatrix(int n)
    {
        return Matrix<Complex>.Build.Random(2, 2);
    }

    public bool Equals(Gate otherGate)
    {
        double floatingPointThreshold = 0.001;
        Matrix<Complex> otherMatrix = otherGate.GetMatrix();
        if (this.m_type != otherGate.GetType())
            return false;
        if (this.m_numQubits != otherGate.GetNumQubits())
            return false;
        if (m_matrix.RowCount != otherMatrix.RowCount)
            return false;
        if (m_matrix.ColumnCount != otherMatrix.ColumnCount)
            return false;
        for (int row = 0; row < m_matrix.RowCount; row++)
            for (int col = 0; col < m_matrix.ColumnCount; col++)
            {
                if (Math.Abs(m_matrix[row, col].Real - otherMatrix[row, col].Real) > floatingPointThreshold)
                    return false;
                if (Math.Abs(m_matrix[row, col].Imaginary - otherMatrix[row, col].Imaginary) > floatingPointThreshold)
                    return false;
            }
        return true;
    }
}
