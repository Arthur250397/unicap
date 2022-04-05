using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.ExceptionServices;

namespace Compilador.compilador
{
    public class Lexico
    {
        private char[] conteudo;
        private int indiceConteudo;

        public Lexico(String caminhoFile)
        {
            try
            {
                String conteudoFile;
                conteudoFile = File.ReadAllText(caminhoFile).ToString();
                this.conteudo = conteudoFile.ToCharArray();
                this.indiceConteudo = 0;
            }
            catch(IOException e)
            {
                Console.WriteLine(e.InnerException.ToString());
            }
        } 

        private char proxChar()
        {
            return this.conteudo[this.indiceConteudo++];
        }

        private Boolean hasProxChar()
        {
            return indiceConteudo < this.conteudo.Length;
        }

        private void voltar()
        {
            this.indiceConteudo--;
        }

        private Boolean isLetra(char c)
        {
            return (c >= 'a') && (c <= 'z');
        } 

        private Boolean isDigito(char c)
        {
            return (c >= '0') && (c <=  '9');
        }  

        private Boolean isOperador(char c)
        {
            return isOperadorAritimetico(c) || isOperadorAtribuicao(c) || isOperadorRelacional(c);
        }

        private Boolean isOperadorAritimetico(char c)
        { 
            return (c == '+') || (c == '-') || (c == '*') || (c == '/');
        }   

        private Boolean isOperadorRelacional(char c)
        {
            return (c == '<') || (c == '>') || (c == '!') ;
        }

        private Boolean isOperadorAtribuicao(char c)
        {
            return (c == '=');
        }

        private Boolean isPalavraReservada(string c)
        {
            return  c.Equals("main")  ||
                    c.Equals("if")    ||
                    c.Equals("else")  ||
                    c.Equals("while") ||
                    c.Equals("do")    ||
                    c.Equals("for")   ||
                    c.Equals("int")   ||
                    c.Equals("float") ||
                    c.Equals("char");
        }
        private Boolean isCaracterEspecial(char c)
        {
            return (c == ')') ||
                   (c == '(') ||
                   (c == '{') ||
                   (c == '}') ||
                   (c == ',') ||
                   (c == ';');
        }

        public Token nextToken()
        {
            Token token = null;
            char c;
            int estado = 0;

            StringBuilder lexema = new StringBuilder();
            while (this.hasProxChar())
            {
                c = this.proxChar();
                switch (estado)
                {
                    case 0:
                        if(c == ' ' || c == '\t' || c == '\n' || c == '\r')
                        {
                            estado = 0;
                        }
                        else if(this.isLetra(c) || c == '_')
                        {
                            lexema.Append(c);
                            estado = 1;
                        }
                        else if(this.isDigito(c) || c == '_')
                        {
                            lexema.Append(c);
                            estado = 2;
                        }
                        else if(this.isCaracterEspecial(c))
                        {
                            lexema.Append(c);
                            estado = 5;
                        }
                        else if (this.isOperadorAritimetico(c))
                        {
                            lexema.Append(c);
                            estado = 6;
                        }
                        else if (this.isOperadorAtribuicao(c))
                        {
                            lexema.Append(c);
                            estado = 7;
                        }
                        else if (this.isOperadorRelacional(c))
                        {
                            lexema.Append(c);
                            estado = 8;
                        }
                        else if(c == ':')
                        {
                            lexema.Append(c);
                            estado = 9;
                        }
                        else if (c == '$')
                        {
                            lexema.Append(c);
                            estado = 99;
                            this.voltar();
                        }
                        else
                        {
                            lexema.Append(c);
                            Console.WriteLine("Erro: token invalido \\" + lexema.ToString() + "\\");
                        }
                        break;
                    case 1: 
                        if(this.isLetra(c) || this.isDigito(c) || c == '_')
                        {
                            lexema.Append(c);
                            estado = 1;
                        }
                        else
                        {
                            this.voltar(); 
                            if(this.isPalavraReservada(lexema.ToString()))
                                return new Token(lexema.ToString(), Token.TIPO_PALAVRA_RESERVADA);
                            else
                                return new Token(lexema.ToString(), Token.TIPO_IDENTIFICADOR);
                        }
                        break;
                    case 2:
                        if (this.isDigito(c))
                        {
                            lexema.Append(c);
                            estado = 2;
                        }
                        else if(c == '.')
                        {
                            lexema.Append(c);
                            estado = 3;
                        }
                        else
                        {
                            this.voltar();
                            return new Token(lexema.ToString(), Token.TIPO_INTEIRO);
                        }
                        break;
                    case 3:
                        if (this.isDigito(c))
                        {
                            lexema.Append(c);
                            estado = 4;
                        }
                        else
                        {
                            Console.WriteLine("Erro: número float inválido \\" + lexema.ToString() + "\\");
                        }
                        break;
                    case 4:
                        if (this.isDigito(c))
                        {
                            lexema.Append(c);
                            estado = 4;
                        }
                        else
                        {
                            this.voltar();
                            return new Token(lexema.ToString(), Token.TIPO_REAL);
                        }
                        break;
                    case 5:
                        this.voltar();
                        return new Token(lexema.ToString(), Token.TIPO_CARACTER_ESPECIAL);
                    case 6:
                        this.voltar();
                        return new Token(lexema.ToString(), Token.TIPO_OPERADOR_ARITMETICO);
                    case 7:
                        if (this.isOperadorAtribuicao(c))
                        {
                            lexema.Append(c);
                            estado = 8;
                        }
                        else
                        {
                            this.voltar();
                            return new Token(lexema.ToString(), Token.TIPO_ATRIBUICAO);
                        }
                        break;
                    case 8:
                        if (this.isOperadorAtribuicao(c) && !lexema.ToString().Equals("==="))
                        {
                            lexema.Append(c);
                            estado = 8;
                        }
                        else if(lexema.ToString().Equals(">") ||
                                lexema.ToString().Equals("<") ||
                                lexema.ToString().Equals(">=") ||
                                lexema.ToString().Equals("<=") ||
                                lexema.ToString().Equals("!=") ||
                                lexema.ToString().Equals("=="))
                        { 
                            this.voltar();
                            return new Token(lexema.ToString(), Token.TIPO_OPERADOR_RELACIONAL);
                        }
                        else
                        {
                            Console.WriteLine("Erro: Operador inválido \\" + lexema.ToString() + "\\");
                        }
                        break;
                    case 9:
                        if (c == '3' || c == ')')
                        {
                            lexema.Append(c);
                            estado = 9;
                        }
                        else if (lexema.ToString() == ":3")
                        {
                            this.voltar(); 
                            return new Token(lexema.ToString(), Token.TIPO_CARINHA_FOFA);
                        }
                        else if(lexema.ToString() == ":)")
                        { 
                            this.voltar(); 
                            return new Token(lexema.ToString(), Token.TIPO_CARINHA_FELIZ);
                        }
                        else
                        { 
                            Console.WriteLine("Erro: Emote inválido \\" + lexema.ToString() + "\\");
                        }
                        break;
                    case 99:
                        return new Token(lexema.ToString(), Token.TIPO_FIM_CODIGO);
                }
            } 
            return token;
        }
    }
}
