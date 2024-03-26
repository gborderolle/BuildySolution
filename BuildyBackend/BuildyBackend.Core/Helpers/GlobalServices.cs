using BuildyBackend.Core.DTO;
using NodaTime;
using System.Globalization;
using System.Text;

namespace BuildyBackend.Core.Helpers
{
    public static class GlobalServices
    {
        public static string GetDatetimeUruguayString()
        {
            CultureInfo culture = new("es-UY");
            var nowInUruguay = SystemClock.Instance.GetCurrentInstant().InZone(DateTimeZoneProviders.Tzdb["America/Montevideo"]);
            return nowInUruguay.ToDateTimeUnspecified().ToString("G", culture);
        }

        public static DateTime GetDatetimeUruguay()
        {
            CultureInfo culture = new("es-UY");
            var nowInUruguay = SystemClock.Instance.GetCurrentInstant().InZone(DateTimeZoneProviders.Tzdb["America/Montevideo"]);
            return nowInUruguay.ToDateTimeUnspecified();
        }

        internal static string NormalizeDiacritics(string attribute)
        {
            var normalizedString = attribute.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
        }

        public static string GetEmailNotificationBody(BuilderUserLoginDTO userCredential, string? clientIP, string? clientIPCity, bool isMobile)
        {
            string device = isMobile ? "Móvil" : "PC";
            string body = $"<div><strong>API Web Service</strong></div>";
            body += $"<div>Notificación de acceso a la plataforma.</div>";
            body += "<br/>";
            body += $"<div><strong>Login user: </strong>{Utils.ToCamelCase(userCredential.Username)}</div>";
            body += $"<div><strong>User IP: </strong>{clientIP}</div>";
            body += $"<div><strong>User City: </strong>{clientIPCity}</div>";
            body += $"<div><strong>Dispositivo: </strong>{device}</div>";
            body += "<br/>";
            body += $"<div>Fecha de acceso: {GetDatetimeUruguayString()}</div>";
            body += "<div>Este es un email auto-generado, por favor no lo responda.</div>";
            return body;
        }

    }
}
