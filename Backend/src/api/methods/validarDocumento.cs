namespace api.Methods;

public static class Documento
{
    public static bool ValidarDocumento(string paisDocumento, string tipoDocumento, string numeroDocumento)
    {
        if (string.IsNullOrWhiteSpace(paisDocumento) ||
            string.IsNullOrWhiteSpace(tipoDocumento) ||
            string.IsNullOrWhiteSpace(numeroDocumento))
        {
            return false;
        }

        var pais = Normalizar(paisDocumento);
        var tipo = Normalizar(tipoDocumento);
        var numero = SoloDigitos(numeroDocumento);

        return (pais, tipo) switch
        {
            ("uruguay", "ci") => ValidarCiUruguay(numero),
            ("uruguay", "dni") => ValidarCiUruguay(numero),

            ("argentina", "dni") => numero.Length is >= 7 and <= 8,

            ("brasil", "cpf") => ValidarCpfBrasil(numero),
            ("brazil", "cpf") => ValidarCpfBrasil(numero),

            ("chile", "rut") => ValidarRutChile(numeroDocumento),

            ("paraguay", "ci") => numero.Length is >= 6 and <= 8,
            ("paraguay", "dni") => numero.Length is >= 6 and <= 8,

            ("peru", "dni") => numero.Length == 8,

            ("colombia", "cc") => numero.Length is >= 6 and <= 10,
            ("colombia", "dni") => numero.Length is >= 6 and <= 10,

            ("mexico", "curp") => ValidarCurpMexico(numeroDocumento),

            ("espana", "dni") => ValidarDniEspana(numeroDocumento),
            ("españa", "dni") => ValidarDniEspana(numeroDocumento),

            ("estados unidos", "ssn") => ValidarSsnEstadosUnidos(numero),
            ("usa", "ssn") => ValidarSsnEstadosUnidos(numero),
            ("united states", "ssn") => ValidarSsnEstadosUnidos(numero),

            ("canada", "sin") => ValidarSinCanada(numero),
            ("canadá", "sin") => ValidarSinCanada(numero),

            _ => false
        };
    }

    private static bool ValidarCiUruguay(string numero)
    {
        if (numero.Length < 7 || numero.Length > 8)
            return false;

        numero = numero.PadLeft(8, '0');

        int[] multiplicadores = { 2, 9, 8, 7, 6, 3, 4 };

        var suma = 0;

        for (var i = 0; i < 7; i++)
        {
            suma += ((numero[i] - '0') * multiplicadores[i]) % 10;
        }

        var digitoVerificador = suma % 10 == 0 ? 0 : 10 - suma % 10;

        return numero[7] - '0' == digitoVerificador;
    }

    private static bool ValidarCpfBrasil(string numero)
    {
        if (numero.Length != 11)
            return false;

        if (numero.All(c => c == numero[0]))
            return false;

        var suma = 0;

        for (var i = 0; i < 9; i++)
            suma += (numero[i] - '0') * (10 - i);

        var resto = suma % 11;
        var digito1 = resto < 2 ? 0 : 11 - resto;

        if (numero[9] - '0' != digito1)
            return false;

        suma = 0;

        for (var i = 0; i < 10; i++)
            suma += (numero[i] - '0') * (11 - i);

        resto = suma % 11;
        var digito2 = resto < 2 ? 0 : 11 - resto;

        return numero[10] - '0' == digito2;
    }

    private static bool ValidarRutChile(string rut)
    {
        rut = rut.Replace(".", "").Replace("-", "").Trim().ToUpper();

        if (rut.Length < 2)
            return false;

        var cuerpo = rut[..^1];
        var dv = rut[^1];

        if (!cuerpo.All(char.IsDigit))
            return false;

        var suma = 0;
        var multiplicador = 2;

        for (var i = cuerpo.Length - 1; i >= 0; i--)
        {
            suma += (cuerpo[i] - '0') * multiplicador;
            multiplicador = multiplicador == 7 ? 2 : multiplicador + 1;
        }

        var resultado = 11 - suma % 11;

        var dvEsperado = resultado switch
        {
            11 => '0',
            10 => 'K',
            _ => resultado.ToString()[0]
        };

        return dv == dvEsperado;
    }

    private static bool ValidarDniEspana(string dni)
    {
        dni = dni.Trim().ToUpper();

        if (dni.Length != 9)
            return false;

        var numeros = dni[..8];
        var letra = dni[8];

        if (!numeros.All(char.IsDigit))
            return false;

        const string letras = "TRWAGMYFPDXBNJZSQVHLCKE";

        var numero = int.Parse(numeros);
        var letraEsperada = letras[numero % 23];

        return letra == letraEsperada;
    }

    private static bool ValidarCurpMexico(string curp)
    {
        curp = curp.Trim().ToUpper();

        if (curp.Length != 18)
            return false;

        return curp.All(char.IsLetterOrDigit);
    }

    private static bool ValidarSsnEstadosUnidos(string numero)
    {
        if (numero.Length != 9)
            return false;

        var area = int.Parse(numero[..3]);
        var grupo = int.Parse(numero.Substring(3, 2));
        var serie = int.Parse(numero.Substring(5, 4));

        if (area == 0 || area == 666 || area >= 900)
            return false;

        if (grupo == 0)
            return false;

        if (serie == 0)
            return false;

        return true;
    }

    private static bool ValidarSinCanada(string numero)
    {
        if (numero.Length != 9)
            return false;

        var suma = 0;

        for (var i = 0; i < 9; i++)
        {
            var digito = numero[i] - '0';

            if (i % 2 == 1)
            {
                digito *= 2;

                if (digito > 9)
                    digito -= 9;
            }

            suma += digito;
        }

        return suma % 10 == 0;
    }

    private static string SoloDigitos(string valor)
    {
        return new string(valor.Where(char.IsDigit).ToArray());
    }

    private static string Normalizar(string valor)
    {
        return valor.Trim().ToLower();
    }
}