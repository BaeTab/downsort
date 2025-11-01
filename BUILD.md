# 빌드 가이드

## 빠른 시작

### 개발 환경 설정
```powershell
# 1. DevExpress NuGet source 추가
dotnet nuget add source "https://nuget.devexpress.com/{YOUR_API_KEY}/api/v3/index.json" --name DevExpress

# 2. 패키지 복원
dotnet restore

# 3. 빌드
dotnet build --configuration Release

# 4. 테스트
dotnet test
```

**API Key 획득**: https://nuget.devexpress.com

---

## 설치 프로그램 생성

```powershell
cd Setup
.\build-installer.ps1 -Version "1.0.2" -CreateInstaller
```

**생성 파일**:
- `Installer/DownSort-Setup-{version}.exe`
- `Installer/DownSort-v{version}-win-x64.zip`
- `Installer/checksums.txt`

---

## 릴리스 프로세스

### 1. 버전 업데이트
- `Directory.Build.props` 버전 수정
- `CHANGELOG.md` 업데이트

### 2. 빌드 및 테스트
```powershell
dotnet test
cd Setup
.\build-installer.ps1 -Version "{version}" -CreateInstaller
```

### 3. Git 태그 생성
```sh
git add .
git commit -m "Release v{version}"
git tag -a v{version} -m "Release v{version}"
git push origin master --tags
```

### 4. GitHub Release
1. https://github.com/BaeTab/downsort/releases
2. "Draft a new release"
3. 파일 업로드 및 공개

---

## 문제 해결

### DevExpress 인증 오류
```powershell
dotnet nuget remove source DevExpress
dotnet nuget add source "https://nuget.devexpress.com/{YOUR_API_KEY}/api/v3/index.json" --name DevExpress
dotnet restore --force
```

### 빌드 오류
```powershell
dotnet clean
dotnet restore --force
dotnet build --configuration Release
```

---

## 요구사항

- .NET 8 SDK
- Visual Studio 2022 (권장)
- DevExpress WPF v25.1
- Inno Setup 6 (설치 프로그램용)

**상세 가이드**: `Setup/README.md`
