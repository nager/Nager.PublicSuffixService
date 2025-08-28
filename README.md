# Nager.PublicSuffixService  

**Nager.PublicSuffixService** is an open-source .NET service that provides the [Public Suffix List (PSL)](https://publicsuffix.org/) as a **ready-to-use Web API**.  
It allows you to parse domains, extract effective TLDs, subdomains, and registrable domains, while keeping the PSL up to date.  

## ✨ Features  

- 🌍 Provides the Public Suffix List as a Web API.  
- 🧩 Parse domains into **TLD**, **registrable domain**, and **subdomain**.  
- 🔄 Automatic updates of PSL rules from [publicsuffix/list](https://github.com/publicsuffix/list).  
- ⚡ Rate-limited endpoints for commit checks and rule updates.  
- 🚀 Easy deployment via **Docker**.  

## 📦 Installation (Docker)  

The service is available as a prebuilt Docker image:  

```bash
docker pull ghcr.io/nager/nager.publicsuffixservice:latest
````

Run the container:

```bash
docker run -d -p 8080:80 ghcr.io/nager/nager.publicsuffixservice:latest
```

The API will now be available at:

```
http://localhost:8080
```

## 🚀 API Endpoints

### 🔹 `GET /DomainInfo/{domain}`

Parses a given domain and returns details such as effective TLD, registrable domain, and subdomain.

**Example:**

```http
GET /DomainInfo/example.co.uk
```

**Response:**

```json
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

### 🔹 `POST /CheckLastCommit`

Checks the latest commit timestamp of the [publicsuffix/list](https://github.com/publicsuffix/list).
Rate-limited to **1 request per minute**.

**Response:**

```
2025-08-28T07:13:21Z
```

### 🔹 `POST /UpdateRules`

Forces an update of the public suffix rules from the upstream repository.
Rate-limited to **1 request every 10 minutes**.

**Response:**

```
204 No Content
```

## 📜 License

This project is licensed under the **MIT License** – free to use, modify, and distribute.
