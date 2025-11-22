# Coders CLI

Coders는 다양한 플랫폼의 네이티브 코드로 변환하는 강력한 트랜스파일러이며, 생성형 AI를 활용해 거의 모든 주요 프로그래밍 언어를 상호 변환하는 도구입니다.

## 설치

```sh
dotnet tool install -g coders
```

## 사용법

### 워크스페이스 초기화

`coders init`을 실행해 기본 설정과 샘플 `.jssp` 파일 묶음을 생성합니다.

- `-f|--force`가 없으면 `config.yml`이 있을 때 작업을 중단합니다.
- `-v|--verbose`로 콘솔 로그를 정보 레벨로 확장합니다.
- `--force`를 사용하면 기존 파일을 새 템플릿으로 덮어씁니다.

생성되는 `config.yml` 기본값은 다음과 같습니다:

```yaml
llmOptions:
  # 사용할 LLM 공급자
  provider: "ollama"
  # 빌드 시 사용할 모델 이름
  model: "gpt-oss-safeguard:20b"
  # LLM 서비스 엔드포인트와 인증 정보
  url: "http://localhost:11434"
  apiKey: "OLLAMA_API_KEY"
  timeoutSeconds: 300
  stream: true
entry: main.jssp
projects: []
```

루트의 `llmOptions`는 LLM 호출에 필요한 공급자, 모델, 엔드포인트, 인증, 시간 제한, 스트리밍 설정을 담당합니다. `projects`는 플랫폼별 산출물을 정의하며, `coders platform add`로 항목을 추가한 뒤 수정하면 됩니다.

### 플랫폼 관리

- `coders platform list [-v]`: 등록된 플랫폼 키와 기본 타깃/언어 정보를 출력합니다.
- `coders platform add <platform> [-v]`: 지정한 플랫폼의 기본 프로젝트 구성을 `config.yml`에 추가합니다. 생성된 엔트리/출력 경로와 옵션은 필요에 따라 편집하면 됩니다.
- `coders platform remove <platform> [-v]`: 예약된 하위 명령입니다.

### 소스 빌드

`coders build <platform> [-v]`로 `.jssp`를 파싱하고 지정한 플랫폼에 맞춰 코드를 생성합니다.

- `platform`은 `config.yml`의 `projects` 항목에 선언된 플랫폼 키와 일치해야 합니다.
- `config.yml`이 없으면 빌드가 중단되며, 엔트리 파일이 없으면 오류가 발생합니다.
- 출력 디렉터리는 없으면 자동으로 만들어집니다.

### 프롬프트 추출

`coders extract <target> <platform> [-o|--output <path>] [-v]`로 프롬프트 등 리소스를 추출합니다.

- `target`은 추출 대상(예: `prompt`), `platform`은 플랫폼 키입니다.
- `--output`으로 결과를 저장할 디렉터리를 바꿀 수 있으며, 지정하지 않으면 현재 위치가 사용됩니다.

### 대표 흐름

```sh
coders init
coders platform add <platform>
coders build <platform> -v
```

초기화 후 필요한 플랫폼을 구성에 추가하고, 동일한 키로 빌드를 실행하는 순서입니다.

## 구성 메모

`config.yml`은 CLI 동작 전반을 제어합니다.

- `entry`: 기본으로 파싱할 루트 `.jssp` 파일입니다.
- `projects`: 각 항목에 `platform`, `name`, `outPath`, `entry`, `target`, `language`, `options`를 정의합니다. `options`에는 `package`, `namespace`, `module`, `mainClass`, `languageVersion`, `version`, `group`, `description`, `plugins`, `dependencies`, `onlySource`, `extra`, `useHistory` 등을 플랫폼 요구에 맞춰 추가할 수 있습니다.
- `llmOptions`: 공급자(`provider`), 모델(`model`), 엔드포인트(`url`), 인증(`apiKey`), 시간 제한(`timeoutSeconds`), 스트리밍(`stream`)을 설정합니다. 추가로 필요한 필드는 같은 블록에 자유롭게 확장할 수 있습니다.

## 문법 하이라이트

Coders 언어는 애플리케이션 로직, HTTP 엔드포인트, 영속성 정의를 단일 `.jssp` 계층에서 다룹니다. 아래 예시는 CLI가 지원하는 일반 패턴입니다.

- **핵심 스크립트**는 표현식, 조건문, 반복문, try/catch, 제네릭, 동적 타이핑을 지원합니다. 타입은 인라인 선언 후 필요에 따라 변환할 수 있습니다.

  ```js
  func main() {
    var numbers list<int32> = [1, 2, 3];
    for (var item in numbers) {
      if (item % 2 == 0) {
        @console.log(@json.encode(item));
      }
    }
  }

  var dynamicValue dynamic = 10;
  dynamicValue = "now a string";
  ```

- **HTTP 컨트롤러**는 라우트, 메서드, 파라미터, 바인딩을 기술하며, 데코레이터로 생성된 클라이언트·서버에 대한 메타데이터를 제공합니다.

  ```js
  [baseUrl='/api/v1/sample']
  controller SampleController {
    [method=get, route='users/{id}', contentType='application/json']
    handler getUser(@path("id") id string, @param includeDetails bool?) UserResponse {
      return UserResponse();
    }
  }
  ```

  재사용 가능한 API 클라이언트는 컨트롤러를 감쌀 수 있습니다.

  ```js
  api SampleApi from @controller.SampleController {
    var server string;
  }

  func main(args list<string>) {
    var api = @api.SampleApi();
    api.server = "http://localhost:8080";
    api.getUser("123", true);
  }
  ```

- **데이터 모델링**은 테이블, 엔티티, 도메인, 매퍼를 연결해 관계형 워크플로를 정의합니다.

  ```js
  domain Email varchar(320);

  table user_profile {
    user_id bigint auto;
    email Email unique;
    key(user_id);
  }

  entity UserProfile {
    var userId bigint;
    var email Email;
  }

  mapper UserProfile {
    query selectByEmail(email Email) UserProfile {
      select user_id, email from user_profile
      where email = :email;
    }
  }
  ```

  스키마 선언은 인덱스와 외래 키 헬퍼도 제공합니다.

  ```js
  table user_role {
    user_id bigint;
    role_id bigint;
    key(user_id, role_id);
    link(user_id) to user(user_id);
  }
  ```

- **매퍼**는 선언된 테이블/엔티티에 대해 select/insert/update/delete 문을 래핑하며, 이름 기반 파라미터(`:name`)를 사용해 바인딩합니다.

  ```js
  mapper TodoMapper {
    query insertTodo(title Title, completed YesNo) int {
      insert into tb_todo (title, completed, created_at, updated_at)
      values (:title, :completed, now(), now());
    }

    query updateTodo(id bigint, completed YesNo) int {
      update tb_todo
      set completed = :completed, updated_at = now()
      where id = :id;
    }

    query deleteTodo(id bigint) int {
      delete from tb_todo where id = :id;
    }
  }
  ```

- **프레젠테이션 헬퍼**는 현지화, 스타일 합성, 프로퍼티 번들을 다룹니다.

  ```js
  define message [locale='en', default=true] {
    welcome 'Hello {name}!';
  }

  define css {
    text.primary {
      text-gray-800 dark:text-gray-100;
    }
  }

  property dev {
    api.url = "https://dev.example.com";
  }
  ```

  런타임에는 `@message`, `@css`, `@property` 매크로로 리소스를 참조합니다.

- **HTML 컴포넌트**는 Vue 스타일 템플릿과 선택적 스크립트 로직을 정의하며, `@css` 같은 헬퍼를 재사용할 수 있습니다.

  ```js
  define css {
    text.primary {
      text-gray-800 dark:text-gray-100;
    }
  }

  html SampleCard {
    var title = "Hello, World!";
    var items = [1, 2, 3];

    <template>
      <div class="@css.text.primary">
        <h1>{{ title }}</h1>
        <p v-if="items.length">Total: {{ items.length }}</p>
      </div>
    </template>
  }
  ```

  생성된 마크업은 `text-gray-800`과 같은 유틸리티 클래스를 정규화하고, 커스텀 CSS를 생략하면 영역 한정 스타일 블록을 제공합니다.

- **네임스페이스와 인터페이스**는 플랫폼 개념을 반영하며 메서드 스타일 접근을 허용합니다.

  ```js
  namespace http {
    interface Header {
      func get(key string) string;
    }
  }

  var headers = http.Header();
  headers.get("Accept");
  ```
