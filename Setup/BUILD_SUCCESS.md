# ?? DownSort 설치 프로그램 생성 완료!

## ? 생성된 파일

### 설치 프로그램
```
?? D:\mySource\Downsort\Installer\DownSort-Setup-1.0.0.exe
   크기: 57.48 MB
   형식: Windows Installer (Inno Setup)
```

### ZIP 아카이브
```
?? D:\mySource\Downsort\Installer\DownSort-v1.0.0-win-x64.zip
   크기: 80.59 MB
   형식: 압축 파일 (수동 설치용)
```

---

## ?? 설치 프로그램 사용 방법

### 1. 설치
```
DownSort-Setup-1.0.0.exe 더블클릭
```

### 2. 설치 마법사
1. **언어 선택**: 한국어 / English
2. **설치 경로**: `C:\Program Files\DownSort\` (기본값)
3. **추가 옵션**:
   - ? 바탕화면 아이콘 만들기
   - ? 빠른 실행 아이콘 만들기
   - ? 시스템 시작 시 자동 실행
4. **설치 완료**: ? "DownSort 실행" 체크하고 완료

---

## ?? 설치 프로그램 기능

### ? 자동 설치 항목
- `C:\Program Files\DownSort\Downsort.exe` - 메인 실행 파일
- `%LocalAppData%\DownSort\` - 사용자 데이터 폴더 생성
  - `settings.json` - 설정 파일
  - `rules.json` - 규칙 파일
  - `logs.json` - 로그 파일

### ? 시작 메뉴 항목
- **DownSort** - 애플리케이션 실행
- **제거** - DownSort 제거

### ? 레지스트리 항목
- `HKCU\Software\DownSort Team\DownSort` - 설치 정보
- `HKCU\Software\Microsoft\Windows\CurrentVersion\Run` - 시작 프로그램 (선택 시)

---

## ??? 제거 방법

### 방법 1: 제어판
```
제어판 → 프로그램 → 프로그램 제거 → "DownSort" 선택 → 제거
```

### 방법 2: 시작 메뉴
```
시작 메뉴 → DownSort → 제거
```

### 사용자 데이터 삭제 옵션
제거 시 물어봅니다:
```
사용자 데이터(설정, 규칙, 로그)를 삭제하시겠습니까?
- 예: 모든 데이터 삭제
- 아니오: 재설치 시 데이터 유지
```

---

## ?? 배포 방법

### 개인 사용
1. `DownSort-Setup-1.0.0.exe` 실행
2. 설치 완료!

### 공유
```
DownSort-Setup-1.0.0.exe 파일을 다음 방법으로 공유:
- USB 드라이브
- 이메일 첨부
- 클라우드 드라이브 (Google Drive, OneDrive, etc.)
- 네트워크 공유 폴더
```

### 웹 배포
```
1. GitHub Releases에 업로드
2. 다운로드 링크 제공
3. 사용자가 다운로드 후 실행
```

---

## ?? 시스템 요구사항

### 운영체제
- Windows 10 (64-bit) 이상
- Windows 11 (권장)

### 소프트웨어
- ? .NET 8 Runtime 필요
- 설치되지 않은 경우: 설치 시 다운로드 링크 제공

### 하드웨어
- 디스크 공간: 최소 100 MB
- RAM: 최소 512 MB (권장 1 GB)
- CPU: 1 GHz 이상

---

## ?? 보안

### Windows Defender
처음 실행 시 경고가 나타날 수 있습니다:
```
"Windows의 PC 보호"
→ "추가 정보" 클릭
→ "실행" 버튼 클릭
```

이는 디지털 서명이 없기 때문입니다 (정상).

### 디지털 서명 추가 (선택 사항)
상용 배포 시 코드 서명 인증서 구입 권장:
- DigiCert
- Sectigo
- GlobalSign

---

## ??? 다시 빌드하는 방법

### 간단한 방법
```powershell
cd D:\mySource\Downsort\Setup
.\build.bat
```
메뉴에서 옵션 2 선택

### PowerShell 명령어
```powershell
cd D:\mySource\Downsort\Setup
.\build-installer.ps1 -CreateInstaller
```

### 독립 실행형 버전 (.NET 포함)
```powershell
.\build-installer.ps1 -SelfContained -CreateInstaller
```

---

## ?? 빌드 통계

```
빌드 시간: ~57초
컴파일 경고: 3개 (정상)
  - Architecture x64 deprecated (무시 가능)
  - OnlyBelowVersion 6.1 (Windows Vista 지원 안 함)
  - PrivilegesRequired admin (HKCU 사용)
```

---

## ?? 다음 단계

### 1. 테스트
- [ ] 새 설치 테스트
- [ ] 업그레이드 설치 테스트
- [ ] 제거 테스트
- [ ] .NET Runtime 없는 PC에서 테스트

### 2. 배포
- [ ] README.md에 다운로드 링크 추가
- [ ] GitHub Release 생성
- [ ] CHANGELOG.md 업데이트

### 3. 홍보
- [ ] 스크린샷 추가
- [ ] 사용 가이드 작성
- [ ] 비디오 튜토리얼 제작 (선택)

---

## ?? 지원

### 문제 발생 시
1. `Setup/TROUBLESHOOTING.md` 참조
2. GitHub Issues에 문의
3. 로그 파일 확인: `%LocalAppData%\DownSort\logs.json`

---

## ?? 축하합니다!

**DownSort 설치 프로그램이 성공적으로 생성되었습니다!**

이제 사용자에게 배포하고 피드백을 받아보세요! ??

---

**생성일**: 2024-11-01
**버전**: 1.0.0
**빌드 도구**: Inno Setup 6.5.4
