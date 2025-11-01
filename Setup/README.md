# DownSort 설치 프로그램 생성 가이드

## ?? 요구사항

### 1. Inno Setup 설치
1. [Inno Setup 다운로드](https://jrsoftware.org/isdl.php)
2. Inno Setup 6.x 이상 버전 설치
3. 설치 경로: `C:\Program Files (x86)\Inno Setup 6\`

### 2. .NET SDK
- .NET 8 SDK 필요
- [다운로드](https://dotnet.microsoft.com/download/dotnet/8.0)

---

## ?? 빌드 방법

### 방법 1: 간단한 빌드 (권장)

```powershell
# 기본 빌드 (프레임워크 종속)
.\Setup\build-installer.ps1 -CreateInstaller

# 독립 실행형 빌드 (.NET Runtime 포함)
.\Setup\build-installer.ps1 -SelfContained -CreateInstaller

# 버전 지정
.\Setup\build-installer.ps1 -Version "1.0.1" -CreateInstaller
```

### 방법 2: 수동 빌드

#### 1단계: 애플리케이션 게시
```powershell
# 프레임워크 종속 버전
dotnet publish Downsort\Downsort.csproj `
  --configuration Release `
  --runtime win-x64 `
  --output Downsort\bin\Release\net8.0-windows\publish `
  /p:PublishSingleFile=true

# 독립 실행형 버전 (.NET Runtime 포함)
dotnet publish Downsort\Downsort.csproj `
  --configuration Release `
  --runtime win-x64 `
  --self-contained true `
  --output Downsort\bin\Release\net8.0-windows\publish `
  /p:PublishSingleFile=true `
  /p:PublishTrimmed=true `
  /p:IncludeNativeLibrariesForSelfExtract=true
```

#### 2단계: 설치 프로그램 생성
```powershell
# Inno Setup 컴파일
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" Setup\DownSort.iss
```

---

## ?? 생성되는 파일

### 설치 프로그램
- **위치**: `Installer\DownSort-Setup-{버전}.exe`
- **크기**: 
  - 프레임워크 종속: ~15-20 MB
  - 독립 실행형: ~80-100 MB

### ZIP 아카이브
- **위치**: `Installer\DownSort-v{버전}-win-x64.zip`
- **용도**: 수동 설치용 (설치 프로그램 없이 배포)

---

## ?? 설치 프로그램 기능

### 설치 옵션
- ? 프로그램 파일 설치: `C:\Program Files\DownSort\`
- ? 바탕화면 아이콘 생성 (선택)
- ? 빠른 실행 아이콘 생성 (선택)
- ? 시작 프로그램에 등록 (선택)

### 설치 시 체크사항
- .NET 8 Runtime 설치 여부 확인
- 설치되지 않은 경우 다운로드 페이지 안내
- 사용자 데이터 디렉토리 생성: `%LocalAppData%\DownSort\`

### 제거 옵션
- ? 프로그램 파일 제거
- ? 레지스트리 항목 제거
- ? 사용자 데이터 보존/삭제 선택 가능
  - 설정 파일: `settings.json`
  - 규칙 파일: `rules.json`
  - 로그 파일: `logs.json`

---

## ?? 커스터마이징

### 버전 변경
`Setup\DownSort.iss` 파일에서:
```inno
#define MyAppVersion "1.0.0"  ; 여기를 수정
```

### 회사 정보 변경
```inno
#define MyAppPublisher "Your Company Name"
#define MyAppURL "https://your-website.com"
```

### 아이콘 변경
```inno
SetupIconFile=..\Downsort\Resources\app.ico
```

### 설치 경로 변경
```inno
DefaultDirName={autopf}\{#MyAppName}  ; Program Files\DownSort
```

---

## ?? 빌드 스크립트 옵션

### build-installer.ps1 파라미터

| 파라미터 | 설명 | 기본값 |
|---------|------|--------|
| `-Configuration` | 빌드 구성 | Release |
| `-Runtime` | 대상 런타임 | win-x64 |
| `-SelfContained` | 독립 실행형 빌드 | false |
| `-CreateInstaller` | 설치 프로그램 생성 | false |
| `-Version` | 버전 번호 | 1.0.0 |

### 예제

```powershell
# 1. 기본 빌드 (프레임워크 종속)
.\Setup\build-installer.ps1

# 2. 설치 프로그램 포함
.\Setup\build-installer.ps1 -CreateInstaller

# 3. 독립 실행형 + 설치 프로그램
.\Setup\build-installer.ps1 -SelfContained -CreateInstaller

# 4. 버전 1.2.0으로 빌드
.\Setup\build-installer.ps1 -Version "1.2.0" -CreateInstaller

# 5. Debug 빌드
.\Setup\build-installer.ps1 -Configuration Debug
```

---

## ?? 배포 체크리스트

### 릴리스 전 확인사항
- [ ] 버전 번호 업데이트 (`DownSort.iss`, `Downsort.csproj`)
- [ ] CHANGELOG.md 업데이트
- [ ] 모든 테스트 통과 확인
- [ ] 빌드 성공 확인
- [ ] 설치 프로그램 테스트
  - [ ] 새로 설치
  - [ ] 업그레이드 설치
  - [ ] 제거 테스트
- [ ] .NET Runtime 없는 환경에서 테스트

### 배포 파일
1. **DownSort-Setup-{버전}.exe** (설치 프로그램)
2. **DownSort-v{버전}-win-x64.zip** (수동 설치용)
3. **DownSort-v{버전}-win-x64-standalone.zip** (독립 실행형)

---

## ?? 문제 해결

### Inno Setup을 찾을 수 없음
```
? Inno Setup not found
```
**해결**: Inno Setup 6.x 설치 또는 경로 확인

### .NET SDK를 찾을 수 없음
```
error : The command "dotnet" is not available
```
**해결**: .NET 8 SDK 설치 및 PATH 환경변수 확인

### 빌드 오류
```
error MSB1009: Project file does not exist
```
**해결**: 프로젝트 루트 디렉토리에서 스크립트 실행

### 설치 프로그램 실행 오류
- **증명서 없음**: 디지털 서명 추가 필요 (선택 사항)
- **Windows Defender**: 예외 추가 또는 디지털 서명

---

## ?? 추가 리소스

- [Inno Setup 공식 문서](https://jrsoftware.org/ishelp/)
- [.NET 게시 문서](https://docs.microsoft.com/dotnet/core/deploying/)
- [WPF 배포 가이드](https://docs.microsoft.com/dotnet/desktop/wpf/deployment)

---

## ?? 코드 서명 (선택 사항)

설치 프로그램에 디지털 서명을 추가하려면:

### 1. 인증서 획득
- 상용 인증서: DigiCert, Sectigo 등
- 개인 테스트용: `makecert` 도구

### 2. Inno Setup 설정
```inno
[Setup]
SignTool=signtool
SignedUninstaller=yes
```

### 3. SignTool 설정
```inno
SignTool=signtool sign /f "path\to\certificate.pfx" /p "password" $f
```

---

## ?? 릴리스 노트 작성

`CHANGELOG.md`에 다음 형식으로 작성:

```markdown
## [1.0.0] - 2024-01-15

### Added
- 초기 릴리스
- 파일 자동 정리 기능
- 6개 기본 규칙

### Changed
- N/A

### Fixed
- N/A
```

---

**생성일**: 2024
**작성자**: DownSort Team
**버전**: 1.0
