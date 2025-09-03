erDiagram
    users ||--o{ connected_calendars : "manages"
    users ||--o{ availability_rules : "defines"
    users ||--o{ events : "has"
    users ||--o{ booking_types : "creates"

    connected_calendcalendars ||--o{ events : "sources"
    booking_types ||--o{ bookings : "is type for"
    events |o--|| bookings : "corresponds to"

    users {
        UUID user_id PK
        VARCHAR full_name
        VARCHAR email UK
        VARCHAR password_hash
        VARCHAR timezone
    }
    connected_calendars {
        UUID connection_id PK
        UUID user_id FK
        ENUM provider
        TEXT access_token
        TEXT refresh_token
    }
    availability_rules {
        UUID rule_id PK
        UUID user_id FK
        ENUM type
        INT day_of_week
        DATE specific_date
        TIME start_time
        TIME end_time
    }
    events {
        UUID event_id PK
        UUID user_id FK
        UUID source_connection_id FK
        VARCHAR external_event_id
        TIMESTAMP start_time_utc
        TIMESTAMP end_time_utc
        BOOLEAN is_booking
    }
    booking_types {
        UUID booking_type_id PK
        UUID user_id FK
        VARCHAR title
        VARCHAR slug UK
        INT duration_minutes
    }
    bookings {
        UUID booking_id PK
        UUID event_id FK
        UUID booking_type_id FK
        VARCHAR booker_name
        VARCHAR booker_email
        ENUM status
    }