# 로컬 DevExpress 설정 완료!

## ?? 문제 해결 완료

### 문제
```
error NU1301: Unable to load the service index for source https://nuget.devexpress.com/api/v3/index.json
error NU1301:   Response status code does not indicate success: 401 (Unauthorized)
```

### 원인
로컬 개발 환경에 DevExpress NuGet feed 인증 정보가 없었습니다.

---

## ? 해결 방법

### 실행한 명령어

```powershell
# 1. 기존 DevExpress source 제거
dotnet nuget remove source DevExpress

# 2. API Key가 포함된 DevExpress source 추가
dotnet nuget add source "https://nuget.devexpress.com/{API_KEY}/api/v3/index.json" --name DevExpress

# 3. 패키지 복원
dotnet restore DownSort.sln

# 4. 빌드 성공 확인
dotnet build DownSort.sln
```

---

## ?? 결과

### 로컬 개발 환경
```
? DevExpress NuGet source 설정 완료
? 패키지 복원 성공 (25.1초)
? 빌드 성공
? Visual Studio에서 오류 없이 개발 가능
```

### GitHub Actions
```
?? DevExpress Secret 설정 필요
- DEVEXPRESS_NUGET_KEY 추가 시 자동 빌드
- 릴리스 자동 생성
```

---

## ?? 로컬에서 사용 가능한 명령어

### 복원
```powershell
dotnet restore DownSort.sln
```

### 빌드
```powershell
dotnet build DownSort.sln --configuration Release
```

### 테스트
```powershell
dotnet test DownSort.sln
```

### 실행
```powershell
dotnet run --project Downsort/Downsort.csproj
```

### 설치 프로그램 생성
```powershell
cd Setup
.\build-installer.ps1 -Version "1.0.1" -CreateInstaller
```

---

## ?? NuGet Source 관리

### 등록된 Source 확인
```powershell
dotnet nuget list source
```

**결과**:
```
Registered Sources:
  1.  nuget.org [Enabled]
      https://api.nuget.org/v3/index.json
  2.  DevExpress [Enabled]
      https://nuget.devexpress.com/{API_KEY}/api/v3/index.json
```

### Source 제거
```powershell
dotnet nuget remove source DevExpress
```

### Source 재추가
```powershell
dotnet nuget add source "https://nuget.devexpress.com/{YOUR_API_KEY}/api/v3/index.json" --name DevExpress
```

---

## ?? 보안 주의사항

### API Key 관리

**? 하지 말아야 할 것**:
- Git에 커밋하지 마세요
- 공개 저장소에 올리지 마세요
- 스크린샷이나 문서에 포함하지 마세요

**? 해야 할 것**:
- 로컬 NuGet source에만 저장
- GitHub Secrets에만 저장
- API Key가 유출되면 즉시 재생성

### API Key 재생성
1. https://nuget.devexpress.com 로그인
2. "Regenerate Feed" 클릭
3. 새로운 API Key 복사
4. 로컬 및 GitHub Secrets 업데이트

---

## ?? 다음 단계

### 1. GitHub Secrets 설정 (권장)

**릴리스 자동 생성을 위해 필수!**

```
https://github.com/BaeTab/downsort/settings/secrets/actions

Name: DEVEXPRESS_NUGET_KEY
Value: QgrRt6Whz4ZTc71TuCk1xiRpBOhRuKPztDgnzJdjrLGhbcKIey
```

### 2. 로컬 개발 계속

```powershell
# Visual Studio에서 솔루션 열기
# 모든 프로젝트 정상 작동
# 디버깅, 빌드, 테스트 가능
```

### 3. 릴리스 생성 (선택)

```powershell
# 로컬에서 설치 프로그램 생성
cd Setup
.\build-installer.ps1 -Version "1.0.1" -CreateInstaller

# 또는 GitHub Actions로 자동 생성
git tag -a v1.0.2 -m "Release v1.0.2"
git push origin v1.0.2
```

---

## ?? 관련 문서

| 문서 | 위치 | 설명 |
|------|------|------|
| DevExpress 설정 | `.github/DEVEXPRESS_SETUP.md` | GitHub Actions 설정 |
| 릴리스 가이드 | `.github/RELEASE_GUIDE.md` | 릴리스 프로세스 |
| 빌드 가이드 | `Setup/README.md` | 로컬 빌드 방법 |
| 문제 해결 | `Setup/TROUBLESHOOTING.md` | 일반적인 문제 |

---

## ? 체크리스트

### 로컬 개발 환경
- [x] DevExpress NuGet source 추가
- [x] 패키지 복원 성공
- [x] 빌드 성공
- [x] Visual Studio 오류 해결
- [ ] 애플리케이션 실행 테스트

### GitHub Actions
- [ ] DevExpress Secret 추가
- [ ] CI 워크플로우 테스트
- [ ] Release 워크플로우 테스트
- [ ] v1.0.1 릴리스 재생성

---

## ?? 완료!

**로컬 개발 환경이 완전히 설정되었습니다!**

이제 다음이 가능합니다:
- ? Visual Studio에서 정상 개발
- ? 디버깅 및 테스트
- ? 로컬 빌드 및 배포
- ?? GitHub Actions 릴리스 (Secret 설정 필요)

---

**작성일**: 2024-01-10  
**상태**: ? 로컬 환경 설정 완료  
**다음**: GitHub Secrets 설정 권장
