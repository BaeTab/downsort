# DevExpress 인증 문제 해결 가이드

## ?? 현재 상황

GitHub Actions에서 여전히 401 Unauthorized 오류가 발생하고 있습니다.

---

## ? 해결 방법 (단계별)

### 1단계: GitHub Secrets 확인

https://github.com/BaeTab/downsort/settings/secrets/actions

**필수 Secret**:
- `DEVEXPRESS_NUGET_KEY` - DevExpress API Key

**확인 사항**:
- [ ] Secret 이름이 정확히 `DEVEXPRESS_NUGET_KEY` 인가?
- [ ] Secret 값이 올바른 API Key인가?
- [ ] Secret이 활성화되어 있는가?

---

### 2단계: API Key 재생성 (권장)

이전에 노출된 API Key는 재생성해야 합니다:

1. https://nuget.devexpress.com 로그인
2. **"Regenerate Feed"** 클릭
3. 새로운 API Key 복사
4. GitHub Secrets 업데이트

---

### 3단계: 워크플로우 로그 확인

https://github.com/BaeTab/downsort/actions

**확인할 내용**:
```
Setup DevExpress NuGet source
├─ ? Secret 설정 확인 메시지
├─ ? dotnet nuget add source 성공
└─ ? NuGet sources 목록 표시
```

**오류 패턴**:
```
? DEVEXPRESS_NUGET_KEY secret is not set
? dotnet nuget add source 실패
? 401 Unauthorized
```

---

## ??? 임시 해결책: CI 비활성화

DevExpress Secret 설정이 어렵다면, CI를 일시적으로 비활성화:

### 옵션 A: CI 워크플로우 비활성화

`.github/workflows/ci.yml` 파일 수정:
