# DownSort 설치 프로그램 빠른 시작 가이드

## ?? 5분 만에 설치 프로그램 만들기

### 1단계: 준비물 확인

```powershell
# PowerShell에서 확인
dotnet --version  # .NET 8.0.x 이상
```

### 2단계: Inno Setup 설치

1. https://jrsoftware.org/isdl.php 에서 다운로드
2. 설치 (기본 경로 사용)

### 3단계: 설치 프로그램 생성

```powershell
# 프로젝트 루트 디렉토리로 이동
cd D:\mySource\Downsort

# 설치 프로그램 생성 (한 줄로!)
.\Setup\build-installer.ps1 -CreateInstaller
```

### 4단계: 완료!

생성된 파일 위치:
- `Installer\DownSort-Setup-1.0.0.exe` ← 설치 프로그램
- `Installer\DownSort-v1.0.0-win-x64.zip` ← 수동 설치용

---

## ?? 빌드 옵션 비교

### 프레임워크 종속 (기본, 권장)
```powershell
.\Setup\build-installer.ps1 -CreateInstaller
```
- ? 크기: ~15-20 MB
- ? 빌드 속도: 빠름
- ?? .NET 8 Runtime 필요

### 독립 실행형 (모든 것 포함)
```powershell
.\Setup\build-installer.ps1 -SelfContained -CreateInstaller
```
- ? .NET Runtime 불필요
- ?? 크기: ~80-100 MB
- ?? 빌드 속도: 느림

---

## ?? 자주 사용하는 명령어

```powershell
# 1. 개발 버전 빌드
.\Setup\build-installer.ps1

# 2. 릴리스 버전 + 설치 프로그램
.\Setup\build-installer.ps1 -CreateInstaller

# 3. 버전 지정
.\Setup\build-installer.ps1 -Version "1.2.0" -CreateInstaller

# 4. 독립 실행형
.\Setup\build-installer.ps1 -SelfContained -CreateInstaller
```

---

## ? 설치 프로그램 테스트

1. **새 설치 테스트**
   ```
   Installer\DownSort-Setup-1.0.0.exe 실행
   ```

2. **설치 확인**
   - 시작 메뉴에 "DownSort" 확인
   - `C:\Program Files\DownSort\` 폴더 확인
   - 애플리케이션 실행 테스트

3. **제거 테스트**
   - 제어판 → 프로그램 제거
   - "DownSort" 선택 → 제거

---

## ?? 문제 해결

### "Inno Setup not found"
→ Inno Setup 6.x 설치 필요

### "dotnet command not found"
→ .NET 8 SDK 설치 필요

### "Access denied"
→ PowerShell을 관리자 권한으로 실행

### 빌드는 성공했지만 설치 프로그램이 없음
→ `-CreateInstaller` 파라미터 추가 필요

---

## ?? 도움말

더 자세한 내용은:
- `Setup\README.md` - 전체 가이드
- `Setup\DownSort.iss` - Inno Setup 스크립트
- `Setup\build-installer.ps1` - 빌드 스크립트

---

**끝! 이제 설치 프로그램을 배포하세요!** ??
