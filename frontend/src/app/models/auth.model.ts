export interface AuthRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
}

export interface JwtPayload {
  sub: string;
  name: string;
  permissions: string[];
  exp: number;
  iss: string;
  aud: string;
}
