# Nager.PublicSuffixService (PSL)

A REST WebApi Docker for the PublicSuffixList

`GET` https://myservicedomain/domaininfo/mail.google.com
```
{
    "domain": "google",
    "topLevelDomain": "com",
    "subdomain": "mail",
    "registrableDomain": "google.com",
    "fullyQualifiedDomainName": "mail.google.com",
    "topLevelDomainRule": {
        "name": "com",
        "type": "Normal",
        "labelCount": 1,
        "division": "ICANN"
    }
}
```
