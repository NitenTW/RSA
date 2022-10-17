Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports RSA
Imports System.Numerics

<TestClass()> Public Class UnitTest1

    <TestMethod()> Public Sub TestMethod1()
        Dim rsa As New RSA.EasyRSA(23, 29)

        Dim data(255) As Byte
        For i = 0 To 255
            data(i) = i
        Next

        Dim encData As BigInteger() = rsa.Encrypt(data)
        Dim decData As Byte() = rsa.Decrypt(encData)

        'data(128) = 0
        For i = 0 To 255
            Assert.AreEqual(data(i), decData(i))
        Next
    End Sub

End Class