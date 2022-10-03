using EncoderConsoleApp.Enums;
using EncoderConsoleApp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EncoderConsoleApp.TratamentoRuido
{
    internal class Crc
    {
        
        private bool[] crc_8_ATM = new bool[] { true , false, false, false, false, false, true, true, true};

        private byte[] cabecalho = new byte[2] ;

        private bool aux = true;


        public byte[] AplicarCrc(byte[] bytes,OperationType operacao) {
            if (operacao==OperationType.Encode)
            {
                byte resto = codifica(bytes);

                //Array.Resize(ref bytes, bytes.Length+1);

                byte[] arrayCrc = new byte[bytes.Length + 1];
                arrayCrc[0] = bytes[0];
                arrayCrc[1] = bytes[1];
                arrayCrc[2] = resto;

                for (int i = 3; i < bytes.Length; i++)
                {
                    arrayCrc[i] = bytes[i]; 
                }

                return arrayCrc;
            }
            else if (operacao == OperationType.Decode)
            {
                byte restoDoEncode = bytes[2];
                byte resto = codifica(bytes);
                if (restoDoEncode != resto)
                {
                    Console.WriteLine("Erro na checagem CRC do cabeçalho do arquivo!!!!");
                    throw new Exception("Erro na checagem CRC ");
                }
                byte[] arrayCrc = new byte[bytes.Length - 1];

                arrayCrc[0] = bytes[0];
                arrayCrc[1] = bytes[1];
                for (int i = 2; i < bytes.Length; i++)
                {
                    arrayCrc[i] = bytes[i];
                }
                return arrayCrc;
            }
            else
            {
                Console.WriteLine("Opção invalida");
                return null;
            }
        }


        private byte codifica(byte[] bytes) {

            cabecalho[0] = bytes[0];
            cabecalho[1] = bytes[1];    
            
            var byte1 = Convert.ToString(cabecalho[0], toBase: 2).PadLeft(8, '0');
            var byte2 = Convert.ToString(cabecalho[1], toBase: 2).PadLeft(8, '0');

            var juncaoBytesShift = (byte1 + byte2).PadRight(32,'0');

            bool[] binario = new bool[juncaoBytesShift.Length];

            for (int i = 0; i < juncaoBytesShift.Length; i++)
            {
                if (juncaoBytesShift.Substring(i, 1) == "1")
                {
                    binario[i] = true;
                }
                else
                {
                    binario[i] = false;
                }
            }

            var qtdBytes = 32;
            bool[] returnXor = xorCrc(binario);

            while (qtdBytes >9)
            {
                returnXor = xorCrc(returnXor);
                if (aux ==false)
                {
                    break;
                }
                qtdBytes = returnXor.Length;
            }

            String rToBinario = "";
            for (int i = 0; i < returnXor.Length; i++)
            {
                if (returnXor[i]==true)
                {
                    rToBinario += '1';
                }
                else
                {
                    rToBinario += '0';
                }
            }
            int resto = Convert.ToInt32(rToBinario,  2);

            return (byte)resto;       

        }

        private byte decodifica(byte[] bytes)
        {

            cabecalho[0] = bytes[0];
            cabecalho[1] = bytes[1];

            var byte1 = Convert.ToString(cabecalho[0], toBase: 2).PadLeft(8, '0');
            var byte2 = Convert.ToString(cabecalho[1], toBase: 2).PadLeft(8, '0');

            var juncaoBytesShift = (byte1 + byte2).PadRight(32, '0');

            bool[] binario = new bool[juncaoBytesShift.Length];

            for (int i = 0; i < juncaoBytesShift.Length; i++)
            {
                if (juncaoBytesShift.Substring(i, 1) == "1")
                {
                    binario[i] = true;
                }
                else
                {
                    binario[i] = false;
                }
            }

            var qtdBytes = 32;
            bool[] returnXor = xorCrc(binario);

            while (qtdBytes > 9)
            {
                returnXor = xorCrc(returnXor);
                if (aux == false)
                {
                    break;
                }
                qtdBytes = returnXor.Length;
            }

            String rToBinario = "";
            for (int i = 0; i < returnXor.Length; i++)
            {
                if (returnXor[i] == true)
                {
                    rToBinario += '1';
                }
                else
                {
                    rToBinario += '0';
                }
            }
            int resto = Convert.ToInt32(rToBinario, 2);

            return (byte)resto;

        }

        private bool[] xorCrc(bool[] b) {
            
            bool[] xor = new bool[b.Length];
            int primeiroBit=0;
            int j = 0;

            for (int i = 0; i < b.Length; i++)
            {
                if (b[i]==true)
                {
                    primeiroBit = i;
                    break;
                }
            }

            if (b.Length-primeiroBit>crc_8_ATM.Length)
            {

            

            for (int i = primeiroBit; j < crc_8_ATM.Length; i++,j++)
            {
                if (b[i]== crc_8_ATM[j])
                {
                    xor[j] = false;
                }
                else
                {
                    xor[j] = true;
                }
            }

            for (int i = primeiroBit+9; i < b.Length; i++,j++)
            {
                xor[j] = b[i];

            }
            Array.Resize<bool>(ref xor, xor.Length - primeiroBit );
            }
            else
            {
                aux = false;
                return b;
            }
            return xor;
        }

    }

    
}
