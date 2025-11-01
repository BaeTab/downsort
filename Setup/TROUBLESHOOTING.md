# DownSort - 설치 프로그램 빠른 생성 가이드

## ?? 중요: 순서대로 실행하세요!

### 1단계: 애플리케이션 게시 (필수!)

먼저 애플리케이션을 빌드하고 게시해야 합니다:

```powershell
# 프로젝트 루트 디렉토리로 이동
cd D:\mySource\Downsort

# .NET 애플리케이션 게시
dotnet publish Downsort\Downsort.csproj `
  --configuration Release `
  --runtime win-x64 `
  --self-contained false `
  --output "Downsort\bin\Release\net8.0-windows\win-x64\publish" `
  /p:PublishSingleFile=true `
  /p:IncludeNativeLibrariesForSelfExtract=true
```

### 2단계: 게시 완료 확인

다음 파일이 있는지 확인:
```
D:\mySource\Downsort\Downsort\bin\Release\net8.0-windows\win-x64\publish\Downsort.exe
```

### 3단계: 설치 프로그램 생성

```powershell
# Setup 디렉토리로 이동
cd Setup

# 빌드 스크립트 실행
.\build-installer.ps1 -CreateInstaller
```

---

## ?? 더 간단한 방법: build.bat 사용

```cmd
cd D:\mySource\Downsort\Setup
build.bat
```

메뉴에서 옵션 2 선택:
```
[1] Build only (no installer)
[2] Build + Create installer       ← 이것 선택!
[3] Build self-contained + installer
```

---

## ?? 오류가 발생하면?

### "Downsort.exe not found"
→ 1단계(게시)를 먼저 실행하세요!

### "Inno Setup not found"
→ Inno Setup 6.x 설치: https://jrsoftware.org/isdl.php

### "dotnet command not found"
→ .NET 8 SDK가 설치되어 있는지 확인

---

## ? 성공 확인

설치 프로그램이 다음 위치에 생성됩니다:
```
D:\mySource\Downsort\Installer\DownSort-Setup-1.0.0.exe
```

---

## ?? 주의사항

1. **반드시 순서대로**: 게시 → 설치 프로그램 생성
2. **Visual Studio 빌드는 충분하지 않음**: `dotnet publish` 명령 필요
3. **경로 확인**: 프로젝트 루트에서 실행

---

## ?? 권장 워크플로우

```powershell
# 한 번에 모두 실행
cd D:\mySource\Downsort\Setup
.\build-installer.ps1 -CreateInstaller
```

이 명령어는 자동으로:
1. ? 의존성 복원
2. ? 테스트 실행
3. ? 애플리케이션 게시
4. ? ZIP 아카이브 생성
5. ? 설치 프로그램 생성

모든 것을 처리합니다!
