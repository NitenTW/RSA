Imports System.Numerics

Public Class EasyRSA
#Region "宣告變數"
    Private PrimeList As BigInteger() '質數清單
    Private N As BigInteger = 0       '公開金鑰
    Private L As BigInteger = 0       'p-1 和 q-1 的最小公倍數

    Public Encode As BigInteger = 0   '加密金鑰
    Public Decode As BigInteger = 0   '解密金鑰
#End Region

    Public Sub New(ByVal p As BigInteger, ByVal q As BigInteger)
        If CheckPrime(p, q) Then
            N = p * q
            If Minus(p, q) > 3 And N > 21 Then  '質數太小會無限迴圈
                L = LCM(p - 1, q - 1)
                PrimeList = MakePrimeList(L)
                GetDecKey()
            End If
        End If
    End Sub

    ''' <summary>
    ''' 確認數值是否為質數
    ''' </summary>
    ''' <param name="tmpValue">數字</param>
    ''' <returns>是質數傳回 True，不是傳回 False</returns>
    Public Function IsPrime(ByVal tmpValue As BigInteger) As Boolean
        For i = 2 To tmpValue - 1
            If (tmpValue Mod i) = 0 Then
                Return False
            End If
        Next

        Return True
    End Function

    ''' <summary>
    ''' 解密
    ''' </summary>
    Public Function Decrypt(ByVal parmDecData As BigInteger()) As Byte()
        If Encode <> 0 And Decode <> 0 Then
            Dim rDecData(parmDecData.Length - 1) As Byte

            For i As UInt64 = 0 To rDecData.Length - 1
                rDecData(i) = ByteDecrypt(parmDecData(i))
                'rDecData(i) = BigInteger.ModPow(parmDecData(i), Decode, N)
            Next

            Return rDecData
        End If
    End Function

    Private Function ByteDecrypt(ByVal parmValue As BigInteger) As Byte
        Dim rValue As BigInteger = 1
        Dim tmpDecode As BigInteger = Decode
        Dim flag As BigInteger = Decode Mod 2
        Dim loopFlag As BigInteger
        Dim tmpValue As BigInteger = parmValue
        Dim previousValue As BigInteger

        If flag = 1 Then
            tmpDecode = Decode - 1
            rValue *= tmpValue
        End If

        Do
            previousValue = tmpValue
            tmpValue = CryptMod(tmpValue)

            loopFlag = tmpDecode Mod 2
            If loopFlag = 1 Then
                tmpDecode -= 1
                rValue *= previousValue
            End If

            tmpDecode = inv(tmpDecode, 2)    'tmpDecode \ 2
        Loop Until tmpDecode <= 1

        Return (rValue * tmpValue) Mod N
    End Function

    ''' <summary>
    ''' 加密
    ''' </summary>
    Public Function Encrypt(ByVal parmEncData As Byte()) As BigInteger()
        If Encode <> 0 And Decode <> 0 Then
            Dim tmpEncData(parmEncData.Length - 1) As BigInteger

            For i As UInt64 = 0 To tmpEncData.Length - 1
                tmpEncData(i) = ByteEncrypt(parmEncData(i))
                'tmpEncData(i) = BigInteger.ModPow(parmEncData(i), Encode, N)
            Next

            Return tmpEncData
        End If
    End Function

    Private Function ByteEncrypt(ByVal parmValue As Byte) As BigInteger
        Dim loopFlag As BigInteger
        Dim tmpEncode As BigInteger = Encode
        Dim saveValue As BigInteger = parmValue
        Dim previousValue As BigInteger
        Dim rValue As BigInteger = 1
        Dim flag As BigInteger = Encode Mod 2

        If flag = 1 Then
            tmpEncode = Encode - 1
            rValue *= saveValue
        End If

        Do
            previousValue = saveValue

            loopFlag = tmpEncode Mod 2
            If loopFlag = 1 Then
                tmpEncode -= 1
                rValue *= previousValue
            End If

            saveValue = CryptMod(saveValue)
            tmpEncode = inv(tmpEncode, 2)    'tmpEncode \ 2
        Loop Until tmpEncode <= 1

        If flag = 1 Then
            Return (rValue * saveValue) Mod N
        End If

        Return rValue
    End Function

    Private Function inv(ByVal value1 As BigInteger, ByVal value2 As BigInteger) As BigInteger
        Dim result As BigInteger = value1 Mod value2
        result = value1 - result
        Return result / value2
    End Function

    Private Function CryptMod(ByVal parmValue As BigInteger) As BigInteger
        Return (parmValue * parmValue) Mod N
    End Function

    ''' <summary>
    ''' 驗證輸入的數值是否為質數
    ''' </summary>
    ''' <returns>是傳回真，不是傳回假</returns>
    Private Function CheckPrime(ByVal tmpValue As BigInteger, ByVal tmpValue2 As BigInteger) As Boolean
        If Not IsPrime(tmpValue) Then
            Return False
        End If

        If Not IsPrime(tmpValue2) Then
            Return False
        End If

        Return True
    End Function

    ''' <summary>
    ''' 將2數加減，取絕數值
    ''' </summary>
    ''' <returns>傳回正數值</returns>
    Private Function Minus(ByVal parmValue As BigInteger, ByVal parmValue2 As BigInteger) As BigInteger
        Return BigInteger.Abs(parmValue - parmValue2)
    End Function

    ''' <summary>
    ''' 求出最大公因數
    ''' </summary>
    Private Function GCD(ByVal parmValue As BigInteger, ByVal parmValue2 As BigInteger) As BigInteger
        Dim tmpValue As BigInteger

        While parmValue2 <> 0
            tmpValue = parmValue Mod parmValue2
            parmValue = parmValue2
            parmValue2 = tmpValue
        End While

        Return parmValue
    End Function

    ''' <summary>
    ''' 求出最小公倍數
    ''' </summary>
    Private Function LCM(ByVal parmValue As BigInteger, ByVal parmValue2 As BigInteger) As BigInteger
        Return parmValue * parmValue2 / GCD(parmValue, parmValue2)
    End Function

    ''' <summary>
    ''' 製作質數表
    ''' </summary>
    ''' <param name="parmValue">此數字以下的質數</param>
    ''' <returns>傳回陣列資料</returns>
    Private Function MakePrimeList(ByVal parmValue As BigInteger) As BigInteger()
        Dim count As BigInteger = 0

        For i As BigInteger = 2 To parmValue
            If IsPrime(i) Then
                count += 1
            End If
        Next

        Dim tmpPrimeList(count - 1) As BigInteger

        Dim arrayCount As BigInteger = 0

        For i As BigInteger = 2 To parmValue
            If IsPrime(i) Then
                tmpPrimeList(arrayCount) = i
                arrayCount += 1
            End If
        Next

        Return tmpPrimeList
    End Function

    ''' <summary>
    ''' 得到加密金鑰
    ''' </summary>
    Private Function GetEncKey(ByVal paramValue As BigInteger) As BigInteger
        Dim k As BigInteger = 0
        Dim tmpValue As BigInteger

        Do
            k += 1
            tmpValue = ((L * k) + 1) Mod paramValue
        Loop Until tmpValue = 0

        tmpValue = ((L * k) + 1) / paramValue

        Return tmpValue
    End Function

    ''' <summary>
    ''' 求出和 L 互質最大的質數(解密金鑰)
    ''' </summary>
    Private Sub GetDecKey()
        Dim count As BigInteger = PrimeList.Length - 1
        Dim tmpValue(count) As BigInteger

        Do
            tmpValue(count) = GetEncKey(PrimeList(count))
            count -= 1
        Loop Until tmpValue(count + 1) <> PrimeList(count + 1)

        If tmpValue(count + 1) = 0 Then MsgBox("0")

        count += 1
        Encode = PrimeList(count)
        Decode = tmpValue(count)
    End Sub
End Class