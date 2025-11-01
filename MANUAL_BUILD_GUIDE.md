# 수동 빌드 가이드

## 개요

DownSort는 **수동 빌드 및 릴리스**를 사용합니다. GitHub Actions를 사용하지 않으므로 모든 빌드는 로컬에서 수행됩니다.

---

## ?? 로컬 개발 환경 설정

### 필수 요구사항
- .NET 8 SDK
- Visual Studio 2022 (권장) 또는 Visual Studio Code
- Inno Setup 6 (설치 프로그램 생성 시)
- DevExpress WPF Components (v25.1.0)

### DevExpress NuGet Source 설정

```powershell
# DevExpress API Key로 NuGet source 추가
dotnet nuget add source "https://nuget.devexpress.com/{YOUR_API_KEY}/api/v3/index.json" --name DevExpress

# 패키지 복원
dotnet restore DownSort.sln
```

**API Key 획득**: https://nuget.devexpress.com

---

## ??? 빌드 방법

### 1. Visual Studio에서 빌드

```
1. DownSort.sln 열기
2. 빌드 구성: Release
3. 솔루션 빌드 (Ctrl+Shift+B)
4. 출력: Downsort/bin/Release/net8.0-windows/
```

### 2. 명령줄에서 빌드

```powershell
# 솔루션 빌드
dotnet build DownSort.sln --configuration Release

# 특정 프로젝트만 빌드
dotnet build Downsort/Downsort.csproj --configuration Release
```

### 3. 테스트 실행

```powershell
# 모든 테스트 실행
dotnet test DownSort.sln --configuration Release

# 상세 출력
dotnet test DownSort.sln --configuration Release --verbosity normal
```

---

## ?? 릴리스 생성

### 방법 1: PowerShell 스크립트 (권장)

```powershell
cd Setup
.\build-installer.ps1 -Version "1.0.2" -CreateInstaller
```

**생성되는 파일**:
- `Installer/DownSort-Setup-1.0.2.exe` - Windows 설치 프로그램
- `Installer/DownSort-v1.0.2-win-x64.zip` - Portable 버전
- `Installer/checksums.txt` - SHA256 체크섬

### 방법 2: 배치 파일

```cmd
cd Setup
build.bat 1.0.2
```

### 방법 3: 수동 단계

#### 3.1 애플리케이션 게시

```powershell
dotnet publish Downsort/Downsort.csproj `
  --configuration Release `
  --runtime win-x64 `
  --self-contained false `
  --output "Downsort/bin/Release/net8.0-windows/publish" `
  /p:PublishSingleFile=true `
  /p:IncludeNativeLibrariesForSelfExtract=true
```

#### 3.2 Inno Setup으로 설치 프로그램 생성

```powershell
# Inno Setup Compiler 실행
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" Setup\DownSort.iss
```

#### 3.3 ZIP 아카이브 생성

```powershell
$version = "1.0.2"
$publishDir = "Downsort/bin/Release/net8.0-windows/publish"
$zipPath = "Installer/DownSort-v$version-win-x64.zip"

Compress-Archive -Path "$publishDir\*" -DestinationPath $zipPath -Force
```

#### 3.4 체크섬 계산

```powershell
$version = "1.0.2"
$setupFile = "Installer/DownSort-Setup-$version.exe"
$zipFile = "Installer/DownSort-v$version-win-x64.zip"

$setupHash = (Get-FileHash -Path $setupFile -Algorithm SHA256).Hash
$zipHash = (Get-FileHash -Path $zipFile -Algorithm SHA256).Hash

@"
Setup SHA256: $setupHash
ZIP SHA256: $zipHash
"@ | Out-File -FilePath "Installer/checksums.txt"
```

---

## ?? GitHub Release 수동 생성

### 1. 버전 업데이트

**Directory.Build.props** 편집:
```xml
<Version>1.0.2</Version>
<AssemblyVersion>1.0.2.0</AssemblyVersion>
<FileVersion>1.0.2.0</FileVersion>
```

### 2. CHANGELOG.md 업데이트

```markdown
## [1.0.2] - 2024-01-10

### Added
- 새로운 기능 설명

### Fixed
- 버그 수정 설명
```

### 3. 커밋 및 태그

```sh
# 변경사항 커밋
git add .
git commit -m "Release v1.0.2"
git push origin master

# 태그 생성
git tag -a v1.0.2 -m "Release v1.0.2"
git push origin v1.0.2
```

### 4. GitHub에서 릴리스 생성

```
1. https://github.com/BaeTab/downsort/releases 이동
2. "Draft a new release" 클릭
3. Tag: v1.0.2 선택
4. Title: v1.0.2 입력
5. Description 작성:
   - 변경 사항
   - 다운로드 링크
   - 시스템 요구사항
6. 파일 업로드:
   - DownSort-Setup-1.0.2.exe
   - DownSort-v1.0.2-win-x64.zip
   - checksums.txt
7. "Publish release" 클릭
```

---

## ?? 버전 번호 규칙

### Semantic Versioning
```
v{Major}.{Minor}.{Patch}
```

**예제**:
- `v1.0.0` - 초기 릴리스
- `v1.0.1` - 버그 수정
- `v1.1.0` - 새 기능
- `v2.0.0` - 주요 변경

---

## ?? 빌드 검증

### 빌드 전 체크리스트
- [ ] DevExpress NuGet source 설정됨
- [ ] Directory.Build.props 버전 업데이트
- [ ] CHANGELOG.md 업데이트
- [ ] 모든 테스트 통과
- [ ] 로컬 빌드 성공

### 빌드 후 체크리스트
- [ ] 설치 프로그램 생성 확인
- [ ] ZIP 파일 생성 확인
- [ ] 체크섬 파일 생성 확인
- [ ] 설치 프로그램 실행 테스트
- [ ] 애플리케이션 실행 테스트

---

## ??? 문제 해결

### DevExpress 패키지 복원 실패

**오류**:
```
error NU1301: Unable to load the service index for source https://nuget.devexpress.com/api/v3/index.json
```

**해결**:
```powershell
# DevExpress source 재설정
dotnet nuget remove source DevExpress
dotnet nuget add source "https://nuget.devexpress.com/{YOUR_API_KEY}/api/v3/index.json" --name DevExpress
```

### Inno Setup 오류

**오류**:
```
Error: Inno Setup not found
```

**해결**:
```powershell
# Chocolatey로 설치
choco install innosetup

# 또는 직접 다운로드
# https://jrsoftware.org/isdl.php
```

### 빌드 오류

**오류**:
```
error CS0246: The type or namespace name 'DevExpress' could not be found
```

**해결**:
```powershell
# 패키지 복원
dotnet restore DownSort.sln --force

# 빌드 재시도
dotnet build DownSort.sln --configuration Release
```

---

## ?? 관련 문서

| 문서 | 위치 | 설명 |
|------|------|------|
| 빌드 스크립트 | `Setup/build-installer.ps1` | PowerShell 빌드 스크립트 |
| 빠른 시작 | `Setup/QUICKSTART.md` | 빠른 빌드 가이드 |
| 문제 해결 | `Setup/TROUBLESHOOTING.md` | 일반적인 문제 해결 |
| DevExpress 설정 | `.github/DEVEXPRESS_SETUP.md` | DevExpress 설정 방법 |

---

## ? 빌드 프로세스 요약

```
1. 버전 업데이트 (Directory.Build.props)
2. CHANGELOG.md 업데이트
3. 로컬 빌드 및 테스트
4. 설치 프로그램 생성 (build-installer.ps1)
5. 파일 검증 (체크섬 확인)
6. Git 커밋 및 태그
7. GitHub Release 수동 생성
8. 파일 업로드
9. 릴리스 공개
```

---

**참고**: 이 프로젝트는 CI/CD를 사용하지 않습니다. 모든 빌드와 릴리스는 수동으로 수행됩니다.

---

**작성일**: 2024-01-10  
**버전**: 1.0  
**작성자**: BaeTab
