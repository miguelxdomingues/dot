
<!-- REFERENCES -->

[Standard_format_specifiers]: https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings

[Custom_format_specifiers]: https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings

[Time_And_Date]: https://www.timeanddate.com/time/zone/portugal/lisbon

[Number_TimeZones]: https://www.timeanddate.com/time/current-number-time-zones.html

<!-- TITLE -->

# DateTime?
Miguel Domingues, Dezembro 2019

<!-- ABSTRACT -->

## Resumo

Os tipos e as variáveis são os conceitos mais básicos de qualquer linguagem de programação e como são tão fáceis de aprender são muitas vezes usados indevidamente. É o caso da Data que, apesar de parecer muito simples, é o tipo de valor que requer alguma atenção quando estamos a construir software para a Cloud. Compreender como funciona o Relógio do Mundo e destacar os aspetos da Data que podem dar origem a anomalias no nosso software é o objetivo deste artigo. Se preferires uma breve explicação salta já para a conclusão final que te permitirá decidir se vale a pena ler o artigo completo!

<!-- BODY -->

## Nota Prévia

Todos os exemplos são em `C#`, mas os conceitos são transversais a outras linguagens. Nos exemplos são usados alguns métodos auxiliares que ajudam a simplificar o que se pretende demonstrar, mas é intuitivo o seu propósito e por isso não apresentamos aqui o código. Também por motivos de simplificação, o termo data refere-se sempre a uma data e hora.

## Formatos de Apresentação

Para apresentar uma data ao utilizador podemos usar formatos pré-definidos ([Standard format specifiers][Standard_format_specifiers]) ou formatos próprios da nossa aplicação ([Custom format specifiers][Custom_format_specifiers]). Consideremos que este código foi executado em `06/05/2019 17:23:40`:

<!-- Source: DateTimeFormats -->
```cs
// Get current date

DateTime dt = DateTime.Now;

// Print using standard format specifiers

Output.PrintLine("{0:t}", dt);
Output.PrintLine("{0:d}", dt);
Output.PrintLine("{0:T}", dt);
Output.PrintLine("{0:D}", dt);
Output.PrintLine("{0:f}", dt);
Output.PrintLine("{0:F}", dt);
Output.PrintLine("{0:g}", dt);
Output.PrintLine("{0:G}", dt);
Output.PrintLine("{0:m}", dt);
Output.PrintLine("{0:y}", dt);
Output.PrintLine("{0:r}", dt);
Output.PrintLine("{0:s}", dt);
Output.PrintLine("{0:u}", dt);

/* Output:
17:23
06/05/2019
17:23:40
Wednesday, 05 June 2019
Wednesday, 05 June 2019 17:23
Wednesday, 05 June 2019 17:23:40
06/05/2019 17:23
06/05/2019 17:23:40
June 05
2019 June
Wed, 05 Jun 2019 17:23:40 GMT
2019-06-05T17:23:40
2019-06-05 17:23:40Z
*/
```

É importante perceber que, qualquer que seja o formato usado, o efeito é apenas visual e não existe nunca uma conversão do valor interno da data para o valor que é apresentado. Trata-se duma extração formatada das várias componentes da data. Mas também podemos usar um formato próprio e para isso podemos usar os literais pré-definidos juntamente com o nosso texto delimitado por uma pelica `'`:

<!-- Source: DateTimeFormats -->
```cs
// Print using custom format specifiers

string df1 = "'Date:' yyyy'-'MM'-'dd', Time:' HH':'mm':'ss.fff', Offset: 'zzz";
string df2 = "yyyy'-'MM'-'dd HH':'mm':'ss.fff zzz";

Output.PrintLine(dt.ToString(df1));
Output.PrintLine(dt.ToString(df2));

/* Output:
Date: 2019-06-05, Time: 17:23:40.540, Offset: +01:00
2019-06-05 17:23:40.540 +01:00
*/
```

Se reparamos no exemplo anterior, para além dos literais do ano, mês, dia, hora, minutos, segundos e milisegundos, também aplicamos o literal `zzz` para nos apresentar o valor do _Offset_ - a diferença para o fuso horário de referência, conhecido por **Coordinated Universal Time (UTC)**, a partir do qual se calculam todas as zonas horárias do mundo. 

Para percebermos melhor este e outros conceitos temos de entrar na história da medição do tempo e encaixar uma parte teórica que é um bocadinho aborrecida, mas que nos ajudará a não cometer erros quando estivermos a construir software para o mundo global.

## O Relógio do Mundo

Todos os fusos horários são definidos em relação ao **Coordinated Universal Time (UTC)**, o fuso horário de Londres quando esta cidade não está no horário de verão e onde se localiza o conhecido meridiano de Greenwich. As zonas horárias, ou fusos horários, são cada uma das 24 regiões em que se divide a Terra e que seguem a mesma referência temporal. Os fusos horários estão centrados nos meridianos das longitudes, mas as áreas não são necessariamente coincidentes devido a questões geo-políticas. 

O mundo está então organizado em **TimeZones** que traduzem a ideia de que existe uma janela temporal que acompanha a rotação da Terra e o movimento da luz solar sobre a superfície terrestre. Cada zona define um espaço de longitudes (mais ou menos regular) dentro do qual vigora o mesmo horário, o mesmo diferencial de tempo face ao UTC, e é por isso que os registos temporais dentro da mesma zona são comparáveis sem que seja necessário fazer uma conversão de datas (fazendo uma analogia com o sistema métrico, só podemos comparar dois valores depois de os converter para a mesma unidade). Cada _Timezone_ possui um horário uniforme e legalmente determinado, geralmente chamado de **Local Time**. 

E se fosse só isto era demasiado simples! Acontece que, ao longo do ano, ainda se aplica um calendário conhecido por **Daylight Saving Time (DST)** que define os reajustes temporais dentro de cada **Timezone**. Esse calendário define os períodos em que se adianta ou atrasa 1 hora, normalmente em função da estação do ano, para que se possa aproveitar o máximo de luz solar para o horário de laboração, para que as tardes tenham mais luz do dia do que as manhãs.

E para complicar ainda mais, temos o exemplo de **Portugal** que está sujeito a **2 Timezones**, uma sobre o **Território Continental** e outra sobre os **Açores**. Para além da diferença de _Offsets_ entre as 2 _Timezones_, durante o ano ainda ocorrem 2 períodos de mudança de hora, em Março e Outubro, onde o _Offset_ de cada _Timezone_ é ajustado com +/- 1 hora. Curiosamente, estes ajustes de calendário dentro da mesma _Timezone_ não ocorrem exatamente na mesma altura ao longo dos anos. Podemos consultar no site [Time And Date][Time_And_Date] quando é que essas alterações ocorreram ou vão ocorrer:

**Time Changes in Lisbon Over the Years**  

Year | Date & Time | Abbreviation | Time Change | Offset After
:--- | :--- | :--- | :--- | :---
2018 | Sun, 25 Mar, 01:00 | WET → WEST | +1 hour (DST start) | UTC+1h
2018 | Sun, 28 Oct, 02:00 | WEST → WET | -1 hour (DST end) | UTC
2019 | Sun, 31 Mar, 01:00 | WET → WEST | +1 hour (DST start) | UTC+1h
2019 | Sun, 27 Oct, 02:00 | WEST → WET | -1 hour (DST end) | UTC
2020 | Sun, 29 Mar, 01:00 | WET → WEST | +1 hour (DST start) | UTC+1h
2020 | Sun, 25 Oct, 02:00 | WEST → WET | -1 hour (DST end) | UTC
2021 | Sun, 28 Mar, 01:00 | WET → WEST | +1 hour (DST start) | UTC+1h
2021 | Sun, 31 Oct, 02:00 | WEST → WET | -1 hour (DST end) | UTC

_Daylight Saving Time (DST) changes do not necessarily occur on the same date every year._

Este sistema de medição do tempo em vários locais da Terra cria algumas dificuldades no tratamento e registo de datas do software dito global. Por exemplo, quando mudamos rapidamente de zona ao viajar de avião, ou quando marcamos um evento com participantes de vários países. O software deve ser capaz de converter datas entre zonas distintas e de as comparar corretamente.

Por isso, surgiu a necessidade de termos uma zona de referência **Coordinated Universal Time (UTC)**, uma linha temporal universal para a qual se podem projetar todas as datas de qualquer _TimeZone_ através da aplicação de um **Offset** que nos dá a diferença temporal entre a convenção UTC e o tempo observado em qualquer lugar da Terra. A partir do momento em que projetamos as datas sobre a mesma linha temporal, elas são facilmente comparáveis.

**Coordinated Universal Time (UTC) vs. Greenwich Mean Time (GMT)**

É importante distinguir a zona de referência **Coordinated Universal Time (UTC)** da zona **Greenwich Mean Time (GMT)**. Qualquer lugar na Terra é situado pela sua distância a leste ou oeste ao meridiano de Greenwich (0° de longitude) que passa sobre Londres, no Reino Unido. Por convenção, este é também o ponto de referência para o UTC e para cada 15 graus de longitude temos +/- 1 hora de diferença, o chamado UTC _OffSet_. O GMT é apenas uma das mais de 24 _Timezones_ do mundo, isto se cada fuso horário tivesse apenas 1 hora de diferença, mas a **International Date Line (IDL)** ainda acrescenta mais algumas porque existem fusos horários separados por 30 ou 45 minutos, aumentando assim para [38 _TimeZones_][Number_TimeZones]. Confuso? É para estar!

Historicamente, Greenwich é um referencial de observação do tempo que usava telescópios em vez de relógios atómicos, e que foi adotado em 1847 pela ferrovia Britânica. O GMT foi então calibrado pelo **Royal Observatory de Greenwich**, no Reino Unido, para o horário solar médio. O **Universal Time (UT)** é o termo moderno para o sistema internacional baseado em telescópios que veio substituir o GMT desde 1928 de acordo com a **União Astronómica Internacional**. As observações no observatório de Greenwich cessaram em 1954, embora o local ainda seja usado como base para o sistema de coordenadas. 

Hoje em dia, Greenwich é o meridiano a partir do qual se mede a longitude usada nas cartas de navegação terrestre. Na aviação, as autorizações de voo e o tráfego aéreo utilizam o horário UTC para evitar confusões decorrentes dos diferentes fusos e horários de verão, para assegurar que todos os pilotos, independentemente da localização, usam a mesma referência horária. Especialmente na comunicação por rádio, o horário UTC é conhecido como "horário zulu", isto porque, no alfabeto fonético da OTAN, a palavra usada para Z (de zero) é "zulu".

O UTC tornou-se no sistema mais utilizado para gerir o tempo da Internet e da World Wide Web. Foi criado o serviço **Network Time Protocol** para fornecer o tempo UTC. Ao contrário das outras _Timezones_, o UTC não se define pela rotação da terra face à posição do sol nem por questões geo-políticas, é uma medida derivada do **Tempo Atómico Internacional (TAI)**. Quando as frações de segundo não são importantes, o GMT pode ser considerado equivalente ao UTC e usado como alternativa.

## Configuração da Região e da Cultura

Ao definir as _Regional Settings_ da máquina estamos a escolher o fuso horário local (_Local Timezone_) e o formato de data (_Regional Format_) que o sistema operativo vai usar. Esta configuração passa depois para o contexto de execução das aplicações e, normalmente, pode ser consultado da seguinte forma:

<!-- Source: PrintCurrentInfo -->
```cs
Output.PrintLine("Local Timezone: {0}", TimeZoneInfo.Local.StandardName);
Output.PrintLine("Local Timezone BaseUtcOffset: {0}", TimeZoneInfo.Local.BaseUtcOffset);
Output.PrintLine("Local Timezone Daylight: {0}", TimeZoneInfo.Local.DaylightName);

Output.PrintLine("Current Thread Culture: {0}", Thread.CurrentThread.CurrentCulture.DisplayName);
Output.PrintLine("Current UI Culture: {0}", Thread.CurrentThread.CurrentUICulture.DisplayName);
Output.PrintLine("Current DateTime Format: {0}", DateTimeFormatInfo.CurrentInfo.FullDateTimePattern);

/* Output:
Local Timezone: GMT Standard Time
Local Timezone BaseUtcOffset: 00:00:00
Local Timezone Daylight: GMT Daylight Time

Current Thread Culture: Portuguese (Portugal)
Current UI Culture: English (United States)
Current DateTime Format: dddd, d' de 'MMMM' de 'yyyy HH:mm:ss
*/
```

Já dentro da nossa aplicação podemos alterar o formato da data, mas não podemos alterar o fuso horário local. Por exemplo, se alterarmos a cultura o formato vai sofrer alterações, tal como se pode observar na última linha do resultado:

<!-- Source: SetInvariantCulture -->
```cs
// Change to Invariant culture, which is associated with 
// English language but not with any specific country or region

Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

Output.PrintLine("Current Thread Culture: {0}", Thread.CurrentThread.CurrentCulture.DisplayName);
Output.PrintLine("Current UI Culture: {0}", Thread.CurrentThread.CurrentUICulture.DisplayName);
Output.PrintLine("Current DateTime Format: {0}", DateTimeFormatInfo.CurrentInfo.FullDateTimePattern);

/* Output:
Current Thread Culture: Invariant Language (Invariant Country) 
Current UI Culture: Invariant Language (Invariant Country)      
Current DateTime Format: dddd, dd MMMM yyyy HH:mm:ss
*/
```

Se quisermos consultar mais informação sobre o fuso horário local (_Local TimeZone_) que está atualmente configurado na nossa máquina podemos fazer o seguinte:

<!-- Source: LocalTimeZone -->
```cs

// The output of the TimeZone class varies depending on the time zone in which it runs

const string f1 = "{0,-30}{1}";
const string f2 = "{0,-30}{1:yyyy-MM-dd HH:mm}";

// Get the current local time and year of the local time zone

TimeZone localZoneDeprecated = TimeZone.CurrentTimeZone; // Deprecated
TimeZoneInfo localZone = TimeZoneInfo.Local;             // Alternative

DateTime currentDate = DateTime.Now;
int currentYear = currentDate.Year;

// Display the names for standard time and daylight saving time of the local time zone

Output.PrintLine(f1, "Standard time name:", localZone.StandardName);
Output.PrintLine(f1, "Daylight saving time name:", localZone.DaylightName);

// Display the current date and time and show if they occur in daylight saving time

Output.PrintLine("\n" + f2, "Current date and time:", currentDate);
Output.PrintLine(f1, "Daylight saving time?", localZone.IsDaylightSavingTime(currentDate));

// Get the current Coordinated Universal Time (UTC) and UTC offset

DateTime currentUTC = localZone.ToUniversalTime(currentDate);       // Deprecated
DateTime currentUTC = currentDate.ToUniversalTime();                // Alternative
DateTime currentUTC = TimeZoneInfo.ConvertTimeToUtc(currentDate);   // Alternative
TimeSpan currentOffset = localZone.GetUtcOffset(currentDate);

Output.PrintLine(f2, "Coordinated Universal Time:", currentUTC);
Output.PrintLine(f1, "UTC offset:", currentOffset);

// Get the DaylightTime object for the current year

DaylightTime daylight = localZoneDeprecated.GetDaylightChanges(currentYear); // Deprecated            
AdjustmentRule[] adjustrules = localZone.GetAdjustmentRules();               // Alternative, foreach

// Display the daylight saving time range for the current year

Output.PrintLine("\nDaylight saving time for year {0}:", currentYear);
Output.PrintLine("{0:yyyy-MM-dd HH:mm} to {1:yyyy-MM-dd HH:mm}, delta: {2}", daylight.Start, daylight.End, daylight.Delta);

/* Output:
Standard time name:           GMT Standard Time
Daylight saving time name:    GMT Daylight Time

Current date and time:        2019-12-13 18:12
Daylight saving time?         False

Coordinated Universal Time:   2019-12-13 18:12
UTC offset:                   00:00:00

Daylight saving time for year 2019:
2019-03-31 01:00 to 2019-10-27 02:00, delta: 01:00:00
*/
```

E se quisermos saber quais são as _Timezones_ que estão disponíveis no sistema operativo:

<!-- Source: Timezones -->
```cs
// Get all the system TimeZones

var zones = TimeZoneInfo.GetSystemTimeZones();

foreach (TimeZoneInfo zone in zones)
{
    Output.PrintLine("{0}\t{1}\t{2}",
                zone.BaseUtcOffset.ToString(),
                zone.DisplayName,
                zone.Id);
}

/* Output: (16 of 139)
-02:00:00	(UTC-02:00) Coordinated Universal Time-02	UTC-02
-02:00:00	(UTC-02:00) Mid-Atlantic - Old	Mid-Atlantic Standard Time
-01:00:00	(UTC-01:00) Azores	Azores Standard Time
-01:00:00	(UTC-01:00) Cabo Verde Is.	Cape Verde Standard Time
00:00:00	(UTC) Coordinated Universal Time	UTC
00:00:00	(UTC+00:00) Dublin, Edinburgh, Lisbon, London	GMT Standard Time
00:00:00	(UTC+00:00) Monrovia, Reykjavik	Greenwich Standard Time
00:00:00	(UTC+00:00) Sao Tome	Sao Tome Standard Time
00:00:00	(UTC+01:00) Casablanca	Morocco Standard Time
01:00:00	(UTC+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna	W. Europe Standard Time
01:00:00	(UTC+01:00) Belgrade, Bratislava, Budapest, Ljubljana, Prague	Central Europe Standard Time
01:00:00	(UTC+01:00) Brussels, Copenhagen, Madrid, Paris	Romance Standard Time
01:00:00	(UTC+01:00) Sarajevo, Skopje, Warsaw, Zagreb	Central European Standard Time
01:00:00	(UTC+01:00) West Central Africa	W. Central Africa Standard Time
02:00:00	(UTC+02:00) Amman	Jordan Standard Time
02:00:00	(UTC+02:00) Athens, Bucharest	GTB Standard Time
*/
```

## Estratégia de Conversão

Uma data representada apenas pelos elementos temporais mais básicos - ano, mês, dia, hora, minuto, segundo - está incompleta, não pode ser comparada com outra, e não deve ser transferida entre sistemas de informação distintos. Usando uma analogia com os nomes das classes, para termos uma `Fully Qualified Date` temos de saber em que _Timezone_ foi registada.

Voltando ao nosso exemplo inicial, agora é mais fácil interpretar o resultado obtido:

<!-- Source: DateTimeFormats -->
```cs
string df2 = "yyyy'-'MM'-'dd HH':'mm':'ss.fff zzz";
Output.PrintLine(dt.ToString(df2));

/* Output:
2019-06-05 17:23:40.540 +01:00
*/
```

> De acordo com o DST da TimeZone de Portugal Continental, a 31 de Março de 2019, à 1h da manhã, começou o Horário de Verão; como o instante temporal do nosso exemplo é Junho de 2019 e está dentro do período Horário de Verão, o `Offset` é de `UTC+1h`.

E se quisermos confirmar a mudança de DST em determinado período da nossa _Timezone_, podemos verificar se existe uma alteração do _Offset_ nos dias antes/depois:

<!-- Source: TimeZoneDSTChanges -->
```cs
PrintDateTime(new DateTime(2019, 3, 31, 0, 0, 0));
PrintDateTime(new DateTime(2019, 4,  1, 0, 0, 0));

/* Output:
2019-03-31 00:00:00.000 +00:00
2019-04-01 00:00:00.000 +01:00
*/
```

**Então, se uma data é gerida por um sistema tão complexo, que estratégia devemos seguir para apresentar, transacionar e persistir datas corretamente através da nossa aplicação?**

Que estratégia devemos seguir para comparar duas datas, persistir a data para uso futuro, ou comunicar (API) com outras aplicações e serviços? A estratégia é simples, basta converter a data para UTC e assim garantir que conseguimos converter novamente essa data para qualquer _TimeZone_ do mundo. 

No sentido aplicação/utilizador - quando estamos a mostrar a data ao utilizador - devemos converter a data de UTC para _Local TimeZone_. No sentido utilizador/aplicação, quando a data é obtida do utilizador, ainda antes de a guardar na base de dados ou enviar para um serviço externo, devemos converter a data de _Local TimeZone_ para UTC de forma a preservar a componente da _Timezone_ através do valor do _Offset_.

Na prática, devemos usar a propriedade `Kind` do tipo `DateTime` para sabermos com que data estamos a trabalhar. Existem 3 valores possíveis - `Local`, `Utc`, `Unspecified`:

<!-- Source: DateTimeKind -->
```cs
// Create a specified kind of date and time

DateTime dt1 = new DateTime(2019, 3, 31, 23, 59, 59, 999);
DateTime dt2 = new DateTime(2019, 3, 31, 23, 59, 59, 999, System.DateTimeKind.Local);
DateTime dt3 = new DateTime(2019, 3, 31, 23, 59, 59, 999, System.DateTimeKind.Utc);
DateTime dt4 = DateTime.SpecifyKind(dt1, System.DateTimeKind.Utc);

Output.PrintLine("{0}, Kind: {1}", dt1.ToString(dtf), dt1.Kind);
Output.PrintLine("{0}, Kind: {1}", dt2.ToString(dtf), dt2.Kind);
Output.PrintLine("{0}, Kind: {1}", dt3.ToString(dtf), dt3.Kind);
Output.PrintLine("{0}, Kind: {1}", dt4.ToString(dtf), dt4.Kind);

/* Output:
Date: 2019-03-31, Time: 23:59:59.999, Offset: +01:00, Kind: Unspecified
Date: 2019-03-31, Time: 23:59:59.999, Offset: +01:00, Kind: Local
Date: 2019-03-31, Time: 23:59:59.999, Offset: +00:00, Kind: Utc
Date: 2019-03-31, Time: 23:59:59.999, Offset: +00:00, Kind: Utc
*/
```

Admitindo que estamos a receber uma data proveniente dum serviço externo à nossa aplicação, ou que estamos a ler uma data que foi previamente persistida numa base de dados, vejamos alguns exemplos de como podemos fazer a conversão (_parsing/deserialize_) da sua representação para um objeto do tipo `DateTime` e analisar os valores obtidos. Para isso vamos usar o método `DateTime.Parse`:

<!-- Source: DateParseOverview -->
```cs
(string dts, string desc)[] dateInfo = {
    ("08/18/2018 07:22:16", "String with a date and time component"),
    ("08/18/2018", "String with a date component only"),
    ("8/2018", "String with a month and year component only"),
    ("8/18", "String with a month and day component only"),
    ("07:22:16", "String with a time component only"),
    ("7 PM", "String with an hour and AM/PM designator only"),
    ("2018-08-18T07:22:16.0000000Z", "UTC string that conforms to ISO 8601"),
    ("2018-08-18T07:22:16.0000000-07:00", "Non-UTC string that conforms to ISO 8601"),
    ("Sat, 18 Aug 2018 07:22:16 GMT", "String that conforms to RFC 1123"),
    ("08/18/2018 07:22:16 -5:00", "String with date, time, and time zone information" ) };

foreach (var item in dateInfo)
{
    Output.PrintLine($"{item.desc + ":",-52} '{item.dts}' --> {DateTime.Parse(item.dts)}");
}

/* Output:
String with a date and time component:               '08/18/2018 07:22:16'               --> 08/18/2018 07:22:16
String with a date component only:                   '08/18/2018'                        --> 08/18/2018 00:00:00
String with a month and year component only:         '8/2018'                            --> 08/01/2018 00:00:00
String with a month and day component only:          '8/18'                              --> 08/18/2019 00:00:00
String with a time component only:                   '07:22:16'                          --> 12/13/2019 07:22:16
String with an hour and AM/PM designator only:       '7 PM'                              --> 12/13/2019 19:00:00
UTC string that conforms to ISO 8601:                '2018-08-18T07:22:16.0000000Z'      --> 08/18/2018 08:22:16
Non-UTC string that conforms to ISO 8601:            '2018-08-18T07:22:16.0000000-07:00' --> 08/18/2018 15:22:16
String that conforms to RFC 1123:                    'Sat, 18 Aug 2018 07:22:16 GMT'     --> 08/18/2018 08:22:16
String with date, time, and time zone information:   '08/18/2018 07:22:16 -5:00'         --> 08/18/2018 13:22:16
*/
```

É precisamente no momento em que criamos o objeto `DateTime`, a partir da representação textual da data, que podem ocorrer erros. É nesse momento que a data é convertida para uma data local que tem em conta (caso exista) o _Offset_ da _Timezone_ onde foi registada:

<!-- Source: DateParseDetail -->
```cs
// Custom format specifier

string dtf = "yyyy'-'MM'-'dd HH':'mm':'ss.fff zzz";

// Create a string representation of an ISO 8601 compliant UTC date

string dts = "2019-03-31 23:59:59Z";

// Create the DateTime object

// Notice:
// - parsing uses a format provider and style, or default ones
// - parsing means conversion, so the result may not be as expected

DateTime dt1 = DateTime.Parse(dts);
DateTime dt2 = DateTime.Parse(dts, CultureInfo.InvariantCulture, DateTimeStyles.None);
DateTime dt3 = DateTime.Parse(dts, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
DateTime dt4 = DateTime.Parse(dts, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
DateTime dt5 = DateTime.Parse(dts, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

// Date and time results

Output.PrintLine("{0}, Kind: string", dts);

Output.PrintLine("{0}, Kind: {1}", dt1.ToString(dtf), dt1.Kind);
Output.PrintLine("{0}, Kind: {1}", dt2.ToString(dtf), dt2.Kind);
Output.PrintLine("{0}, Kind: {1}", dt3.ToString(dtf), dt3.Kind);
Output.PrintLine("{0}, Kind: {1}", dt4.ToString(dtf), dt4.Kind);
Output.PrintLine("{0}, Kind: {1}", dt5.ToString(dtf), dt5.Kind);

// Convert date and time back to UTC 
// Notice the change of kind and month value!

DateTime dt1utc = dt1.ToUniversalTime();
DateTime dt2utc = dt2.ToUniversalTime();
DateTime dt3utc = dt3.ToUniversalTime();
DateTime dt4utc = dt4.ToUniversalTime();
DateTime dt5utc = dt5.ToUniversalTime();

// Date and time to UTC results

Output.PrintLine("{0}, Kind: {1}", dt1utc.ToString(dtf), dt1utc.Kind);
Output.PrintLine("{0}, Kind: {1}", dt2utc.ToString(dtf), dt2utc.Kind);
Output.PrintLine("{0}, Kind: {1}", dt3utc.ToString(dtf), dt3utc.Kind);
Output.PrintLine("{0}, Kind: {1}", dt4utc.ToString(dtf), dt4utc.Kind);
Output.PrintLine("{0}, Kind: {1}", dt5utc.ToString(dtf), dt5utc.Kind);

/* Output:

2019-03-31 23:59:59Z, Kind: string

2019-04-01 00:59:59.000 +01:00, Kind: Local
2019-04-01 00:59:59.000 +01:00, Kind: Local
2019-04-01 00:59:59.000 +01:00, Kind: Local
2019-04-01 00:59:59.000 +01:00, Kind: Local
2019-03-31 23:59:59.000 +00:00, Kind: Utc

2019-03-31 23:59:59.000 +00:00, Kind: Utc
2019-03-31 23:59:59.000 +00:00, Kind: Utc
2019-03-31 23:59:59.000 +00:00, Kind: Utc
2019-03-31 23:59:59.000 +00:00, Kind: Utc
2019-03-31 23:59:59.000 +00:00, Kind: Utc
*/
```

Enquanto a formatação é apenas uma questão de apresentação visual, no parsing existe sempre uma conversão da representação textual para o tipo de objeto `DateTime` que estamos a criar. Quando a representação textual é incompleta ou foi erradamente persistida - porque não preservou a informação da _Timezone_ - por vezes obtemos um resultado inesperado. O erro também acontece porque, normalmente, não escrevemos código específico para fazer parsing das datas, o que acontece é que essa conversão ocorre algures no código dos packages ou APIs de programação que usamos, o que para nós passa despercebido. Por exemplo, quando recebemos um JSON à entrada da nossa API e fazemos um _deserialize_ para um objeto estamos a fazer essa conversão; quando usamos a _Entity Framework_ para transacionar dados entre a nossa aplicação e a base de dados, também estamos a fazer essa conversão. Se não tivermos a preocupação em definir corretamente o tipo de dados em que persistimos e transacionamos uma data, o erro vai acontecer.

Finalmente, é muito raro mas pode acontecer, se tivermos um caso de uso em que é necessário trabalhar com _Timezones_ distintas e converter explicitamente as datas, eis um exemplo:

<!-- Source: DateConversion -->
```cs
// Set current culture to "en-US"

var currentCulture = new CultureInfo("en-US");
Thread.CurrentThread.CurrentCulture = currentCulture;
Thread.CurrentThread.CurrentUICulture = currentCulture;

Output.PrintHeader("Challenge:");
PrintCultureInfo(currentCulture);

// Get the local time zone (readonly, from regional settings)

var currentTimeZone = TimeZoneInfo.Local; 
PrintTimeZoneInfo(currentTimeZone);

// Set the working time zone to (UTC-08:00) Pacific Time (US & Canada)

var workingTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"); 
PrintTimeZoneInfo(workingTimeZone);

// Create a string representation of an ISO 8601 compliant UTC date

string dts = "2019-03-31 23:00:00Z";

// Parse the string representation to obtain the DateTime object
// using the current culture and local timezone

DateTime dt1 = DateTime.Parse(dts);
DateTime dt2 = dt1.ToUniversalTime(); 
// or TimeZoneInfo.ConvertTimeToUtc(dt1);
DateTime dt3 = TimeZoneInfo.ConvertTime(dt2, workingTimeZone); 
// or TimeZoneInfo.ConvertTimeFromUtc(dt2, workingTimeZone);
DateTime dt4 = dt3.ToUniversalTime();

// Print the results

PrintDateTime(dts);
PrintDateTime(dt1);
PrintDateTime(dt2);
PrintDateTime(dt3); // Converted to Pacific (-8) and then to Local (+1) => (-7)
PrintDateTime(dt4); // Converted again to UTC (-1) => (-8)

/* Output:
Culture: English (United States)
Timezone: GMT Standard Time (UTC+00:00) Dublin, Edinburgh, Lisbon, London
Timezone: Pacific Standard Time (UTC-08:00) Pacific Time (US & Canada)

2019-03-31 23:00:00Z                String              

2019-04-01 00:00:00.000 +01:00      Local               
2019-03-31 23:00:00.000 +00:00      Utc                 
2019-03-31 16:00:00.000 +01:00      Unspecified
2019-03-31 15:00:00.000 +00:00      Utc                 
*/
```

## Conclusão

"Ui! Há anos que escrevo código impecável e nunca tive de me preocupar com as datas", ou então "Ah! Talvez isto explique o problema com as datas". Se tiveste o primeiro pensamento recomendo-te que leias este artigo completo, ou outro equivalente, pelo menos 1 vez na vida! Se leste o artigo e ficaste ainda mais curioso, consulta as referências. Obrigado!

## Anexos

**System Format Specifiers**

`t`	ShortTimePattern                 `h:mm tt`  
`d`	ShortDatePattern                 `M/d/yyyy`  
`T`	LongTimePattern                  `h:mm:ss tt`  
`D`	LongDatePattern                  `dddd, MMMM dd, yyyy`  
`f`	(combination of D and t)         `dddd, MMMM dd, yyyy h:mm tt`  
`F`	FullDateTimePattern              `dddd, MMMM dd, yyyy h:mm:ss tt`  
`g`	(combination of d and t)         `M/d/yyyy h:mm tt`  
`G`	(combination of d and T)         `M/d/yyyy h:mm:ss tt`  
`m, M`	MonthDayPattern              `MMMM dd`  
`y, Y`	YearMonthPattern             `MMMM, yyyy`  
`r, R`	RFC1123Pattern               `ddd, dd MMM yyyy HH':'mm':'ss 'GMT'`  
`s`	SortableDateTi­mePattern          `yyyy'-'MM'-'dd'T'HH':'mm':'ss`  
`u`	UniversalSorta­bleDateTimePat­tern `yyyy'-'MM'-'dd HH':'mm':'ss'Z'`

## Referências

[Standard date and time format strings](https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings)  
[Custom date and time format strings](https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings)  
[Time Zone Database (IANA)](http://www.iana.org/time-zones)  
[Time changes in Portugal over the years](https://www.timeanddate.com/time/zone/portugal/lisbon)  
[Time.is](https://time.is/)  
[Time.is/UTC](https://time.is/UTC)  
[Tempo Universal Coordenado](https://pt.wikipedia.org/wiki/Tempo_Universal_Coordenado)  
[Fuso Horário](https://pt.wikipedia.org/wiki/Fuso_hor%C3%A1rio)  
[24Timezones](https://24timezones.com/hora_certa.php#/map)
