// збілдити тут, а потім в командному рядку
// mpiexec -n 8 "C:\Users\Таїсія\OneDrive\Univer\3_course\ПЗВПКС\lab6\Lab6\x64\Debug\Lab6.exe"

#include <mpi.h>
#include <iostream>
#include <vector>

using namespace std;

const int N = 16;
const int Pr = 8;
const int H = N / Pr;

void initializeVector(vector<int>&);
void initializeMatrix(vector<vector<int>>&);
void appendToBuffer(vector<int>&, const int&);
void appendToBuffer(vector<int>&, const vector<int>&);
void appendToBuffer(vector<int>&, const vector<vector<int>>&);
void extractFromBuffer(vector<int>&, int&, size_t&);
void extractFromBuffer(vector<int>&, vector<int>&, size_t&, size_t);
void extractFromBuffer(vector<int>&, vector<vector<int>>&, size_t&, size_t, size_t);
vector<int> selectPartsH(const vector<int>&, const vector<int>&);
vector<vector<int>> selectPartsH(const vector<vector<int>>&, const vector<int>&);
vector<vector<int>> multiplyMatrices(const vector<vector<int>>&, const vector<vector<int>>&);
vector<int> multiplyMatrixVector(const vector<vector<int>>&, const vector<int>&);

int main(int argc, char* argv[]) {
	MPI_Init(&argc, &argv);

	int rank, size;
	MPI_Comm_rank(MPI_COMM_WORLD, &rank);
	MPI_Comm_size(MPI_COMM_WORLD, &size);

	vector<int> Z_H;
	vector<int> D_H;
	vector<int> C_H;
	vector<int> B_H;
	vector<vector<int>> MR_H;
	vector<vector<int>> MX;

	if (rank == 0) {
		// Введення даних
		vector<int> B;
		initializeVector(B);
		B_H = selectPartsH(B, { rank });

		// Формує буфер для відправлення
		vector<int> send21;
		auto B_5H = selectPartsH(B, { 1, 2, 3, 4, 5 });
		appendToBuffer(send21, B_5H);

		// Передає буфер рангу 1 
		MPI_Send(&send21[0], static_cast<int>(send21.size()), MPI_INT, 1, 0, MPI_COMM_WORLD);

		// Отримує буфер від рангу 1
		vector<int> recv21(12 * H + N * (N + 3 * H));
		vector<int> Z_3H, D_3H, C_3H;
		vector<vector<int>> MR_3H;
		MPI_Recv(recv21.data(), static_cast<int>(recv21.size()), MPI_INT, 1, 0, MPI_COMM_WORLD, MPI_STATUS_IGNORE);

		// Розпаковує буфер
		size_t index = 0;
		extractFromBuffer(recv21, Z_3H, index, 3 * H);
		extractFromBuffer(recv21, D_3H, index, 3 * H);
		extractFromBuffer(recv21, C_3H, index, 3 * H);
		extractFromBuffer(recv21, MR_3H, index, N, 3 * H);
		extractFromBuffer(recv21, MX, index, N, N);
		Z_H = selectPartsH(Z_3H, { 0 });
		D_H = selectPartsH(D_3H, { 0 });
		C_H = selectPartsH(C_3H, { 0 });
		MR_H = selectPartsH(MR_3H, { 0 });
		auto Z_2H = selectPartsH(Z_3H, { 1, 2 });
		auto D_2H = selectPartsH(D_3H, { 1, 2 });
		auto C_2H = selectPartsH(C_3H, { 1, 2 });
		auto MR_2H = selectPartsH(MR_3H, { 1, 2 });

		// Формує буфер для відправлення
		vector<int> send81;
		auto B_2H = selectPartsH(B, { 6, 7 });
		appendToBuffer(send81, Z_2H);
		appendToBuffer(send81, D_2H);
		appendToBuffer(send81, C_2H);
		appendToBuffer(send81, MR_2H);
		appendToBuffer(send81, MX);
		appendToBuffer(send81, B_2H);

		// Передає буфер рангу 4 
		MPI_Send(&send81[0], static_cast<int>(send81.size()), MPI_INT, 7, 0, MPI_COMM_WORLD);
	}
	else if (rank == 1) {
		// Введення даних
		initializeMatrix(MX);

		// Отримує буфер від рангу 0
		vector<int> recv11(5 * H);
		vector<int> B_5H;
		MPI_Recv(recv11.data(), 5 * H, MPI_INT, 0, 0, MPI_COMM_WORLD, MPI_STATUS_IGNORE);

		// Розпаковує буфер
		size_t index = 0;
		extractFromBuffer(recv11, B_5H, index, 5 * H);
		B_H = selectPartsH(B_5H, { 0 });
		auto B_4H = selectPartsH(B_5H, { 1, 2, 3, 4 });

		// Отримує буфер від рангу 3
		vector<int> recv31(4 * H * (N + 3));
		vector<int> Z_4H;
		vector<int> D_4H;
		vector<int> C_4H;
		vector<vector<int>> MR_4H;
		MPI_Recv(recv31.data(), 4 * H * (N + 3), MPI_INT, 2, 0, MPI_COMM_WORLD, MPI_STATUS_IGNORE);

		// Формує буфер для відправлення
		vector<int> send31;
		appendToBuffer(send31, B_4H);
		appendToBuffer(send31, MX);

		// Передає буфер рангу 2
		MPI_Send(&send31[0], 4 * H + N * N, MPI_INT, 2, 0, MPI_COMM_WORLD);

		// Розпаковує буфер
		index = 0;
		extractFromBuffer(recv31, Z_4H, index, 4 * H);
		extractFromBuffer(recv31, D_4H, index, 4 * H);
		extractFromBuffer(recv31, C_4H, index, 4 * H);
		extractFromBuffer(recv31, MR_4H, index, N, 4 * H);
		Z_H = selectPartsH(Z_4H, { 1 });
		D_H = selectPartsH(D_4H, { 1 });
		C_H = selectPartsH(C_4H, { 1 });
		MR_H = selectPartsH(MR_4H, { 1 });
		auto Z_3H = selectPartsH(Z_4H, { 0, 2, 3 });
		auto D_3H = selectPartsH(D_4H, { 0, 2, 3 });
		auto C_3H = selectPartsH(C_4H, { 0, 2, 3 });
		auto MR_3H = selectPartsH(MR_4H, { 0, 2, 3 });

		// Формує буфер для відправлення
		vector<int> send11;
		appendToBuffer(send11, Z_3H);
		appendToBuffer(send11, D_3H);
		appendToBuffer(send11, C_3H);
		appendToBuffer(send11, MR_3H);
		appendToBuffer(send11, MX);

		// Передає буфер рангу 0
		MPI_Send(&send11[0], 12 * H + N * (N + 3 * H), MPI_INT, 0, 0, MPI_COMM_WORLD);
	}
	else if (rank == 2) {
		// Введення даних
		vector<int> Z, D;
		initializeVector(Z);
		initializeVector(D);
		Z_H = selectPartsH(Z, { rank });
		D_H = selectPartsH(D, { rank });

		// Отримує буфер від рангу 3
		vector<int> recv41(5 * H * (N + 1));
		vector<int> C_5H;
		vector<vector<int>> MR_5H;
		MPI_Recv(recv41.data(), 5 * H * (N + 1), MPI_INT, 3, 0, MPI_COMM_WORLD, MPI_STATUS_IGNORE);

		// Розпаковує буфер
		size_t index = 0;
		extractFromBuffer(recv41, C_5H, index, 5 * H);
		extractFromBuffer(recv41, MR_5H, index, N, 5 * H);
		C_H = selectPartsH(C_5H, { 2 });
		MR_H = selectPartsH(MR_5H, { 2 });
		auto C_4H = selectPartsH(C_5H, { 0, 1, 3, 4 });
		auto MR_4H = selectPartsH(MR_5H, { 0, 1, 3, 4 });

		// Формує буфер для відправлення
		vector<int> send21;
		vector<int> parts = { 0, 1, 6, 7 };
		auto Z_4H = selectPartsH(Z, parts);
		auto D_4H = selectPartsH(D, parts);
		appendToBuffer(send21, Z_4H);
		appendToBuffer(send21, D_4H);
		appendToBuffer(send21, C_4H);
		appendToBuffer(send21, MR_4H);

		// Передає буфер рангу 1
		MPI_Send(&send21[0], 4 * H * (N + 3), MPI_INT, 1, 0, MPI_COMM_WORLD);

		// Отримує буфер від рангу 1
		vector<int> recv21(4 * H + N * N);
		vector<int> B_4H;
		MPI_Recv(recv21.data(), 4 * H + N * N, MPI_INT, 1, 0, MPI_COMM_WORLD, MPI_STATUS_IGNORE);

		// Розпаковує буфер
		index = 0;
		extractFromBuffer(recv21, B_4H, index, 4 * H);
		extractFromBuffer(recv21, MX, index, N, N);
		B_H = selectPartsH(B_4H, { 0 });
		auto B_3H = selectPartsH(B_4H, { 1, 2, 3 });

		// Формує буфер для відправлення
		vector<int> send41;
		parts = { 3, 4, 5 };
		auto Z_3H = selectPartsH(Z, parts);
		auto D_3H = selectPartsH(D, parts);
		appendToBuffer(send41, B_3H);
		appendToBuffer(send41, MX);
		appendToBuffer(send41, Z_3H);
		appendToBuffer(send41, D_3H);

		// Передає буфер рангу 3
		MPI_Send(&send41[0], static_cast<int>(send41.size()), MPI_INT, 3, 0, MPI_COMM_WORLD);
	}
	else if (rank == 3) {
		// Введення даних
		vector<int> C;
		vector<vector<int>> MR;
		initializeVector(C);
		initializeMatrix(MR);
		C_H = selectPartsH(C, { rank });
		MR_H = selectPartsH(MR, { rank });

		// Формує буфер для відправлення
		vector<int> send31;
		vector<int> parts = { 0, 1, 2, 6, 7 };
		auto C_5H = selectPartsH(C, parts);
		auto MR_5H = selectPartsH(MR, parts);
		appendToBuffer(send31, C_5H);
		appendToBuffer(send31, MR_5H);

		// Передає буфер рангу 2 
		MPI_Send(&send31[0], 5 * H * (N + 1), MPI_INT, 2, 0, MPI_COMM_WORLD);

		// Отримує буфер від рангу 2
		vector<int> recv31(9 * H + N * N);
		vector<int> B_3H, Z_3H, D_3H;
		MPI_Recv(recv31.data(), static_cast<int>(recv31.size()), MPI_INT, 2, 0, MPI_COMM_WORLD, MPI_STATUS_IGNORE);

		// Розпаковує буфер
		size_t index = 0;
		extractFromBuffer(recv31, B_3H, index, 3 * H);
		extractFromBuffer(recv31, MX, index, N, N);
		extractFromBuffer(recv31, Z_3H, index, 3 * H);
		extractFromBuffer(recv31, D_3H, index, 3 * H);
		B_H = selectPartsH(B_3H, { 0 });
		Z_H = selectPartsH(Z_3H, { 0 });
		D_H = selectPartsH(D_3H, { 0 });
		auto B_2H = selectPartsH(B_3H, { 1, 2 });
		auto Z_2H = selectPartsH(Z_3H, { 1, 2 });
		auto D_2H = selectPartsH(D_3H, { 1, 2 });

		// Формує буфер для відправлення
		vector<int> send51;
		auto C_2H = selectPartsH(C, { 4, 5 });
		auto MR_2H = selectPartsH(MR, { 4, 5 });
		appendToBuffer(send51, B_2H);
		appendToBuffer(send51, MX);
		appendToBuffer(send51, Z_2H);
		appendToBuffer(send51, D_2H);
		appendToBuffer(send51, C_2H);
		appendToBuffer(send51, MR_2H);

		// Передає буфер рангу 4 
		MPI_Send(&send51[0], static_cast<int>(send51.size()), MPI_INT, 4, 0, MPI_COMM_WORLD);
	}
	else if (rank == 4) {
		// Отримує буфер від рангу 3
		vector<int> recv41(8 * H + N * (N + 2 * H));
		vector<int> B_2H, Z_2H, D_2H, C_2H;
		vector<vector<int>> MR_2H;
		MPI_Recv(recv41.data(), static_cast<int>(recv41.size()), MPI_INT, 3, 0, MPI_COMM_WORLD, MPI_STATUS_IGNORE);

		// Розпаковує буфер
		size_t index = 0;
		extractFromBuffer(recv41, B_2H, index, 2 * H);
		extractFromBuffer(recv41, MX, index, N, N);
		extractFromBuffer(recv41, Z_2H, index, 2 * H);
		extractFromBuffer(recv41, D_2H, index, 2 * H);
		extractFromBuffer(recv41, C_2H, index, 2 * H);
		extractFromBuffer(recv41, MR_2H, index, N, 2 * H);
		B_H = selectPartsH(B_2H, { 0 });
		Z_H = selectPartsH(Z_2H, { 0 });
		D_H = selectPartsH(D_2H, { 0 });
		C_H = selectPartsH(C_2H, { 0 });
		MR_H = selectPartsH(MR_2H, { 0 });
		auto BH = selectPartsH(B_2H, { 1 });
		auto ZH = selectPartsH(Z_2H, { 1 });
		auto DH = selectPartsH(D_2H, { 1 });
		auto CH = selectPartsH(C_2H, { 1 });
		auto MRH = selectPartsH(MR_2H, { 1 });

		// Формує буфер для відправлення
		vector<int> send61;
		appendToBuffer(send61, BH);
		appendToBuffer(send61, MX);
		appendToBuffer(send61, ZH);
		appendToBuffer(send61, DH);
		appendToBuffer(send61, CH);
		appendToBuffer(send61, MRH);

		// Передає буфер рангу 2 
		MPI_Send(&send61[0], static_cast<int>(send61.size()), MPI_INT, 5, 0, MPI_COMM_WORLD);
	}
	else if (rank == 5) {
		// Отримує буфер від рангу 4
		vector<int> recv51(4 * H + N * (N + H));
		MPI_Recv(recv51.data(), static_cast<int>(recv51.size()), MPI_INT, 4, 0, MPI_COMM_WORLD, MPI_STATUS_IGNORE);

		// Розпаковує буфер
		size_t index = 0;
		extractFromBuffer(recv51, B_H, index, H);
		extractFromBuffer(recv51, MX, index, N, N);
		extractFromBuffer(recv51, Z_H, index, H);
		extractFromBuffer(recv51, D_H, index, H);
		extractFromBuffer(recv51, C_H, index, H);
		extractFromBuffer(recv51, MR_H, index, N, H);
	}
	else if (rank == 6) {
		// Отримує буфер від рангу 7
		vector<int> recv81(4 * H + N * (N + H));
		MPI_Recv(recv81.data(), static_cast<int>(recv81.size()), MPI_INT, 7, 0, MPI_COMM_WORLD, MPI_STATUS_IGNORE);

		// Розпаковує буфер
		size_t index = 0;
		extractFromBuffer(recv81, Z_H, index, H);
		extractFromBuffer(recv81, D_H, index, H);
		extractFromBuffer(recv81, C_H, index, H);
		extractFromBuffer(recv81, MR_H, index, N, H);
		extractFromBuffer(recv81, MX, index, N, N);
		extractFromBuffer(recv81, B_H, index, H);
	}
	else if (rank == 7) {
		// Отримує буфер від рангу 0
		vector<int> recv11(8 * H + N * (N + 2 * H));
		vector<int> B_2H, Z_2H, D_2H, C_2H;
		vector<vector<int>> MR_2H;
		MPI_Recv(recv11.data(), static_cast<int>(recv11.size()), MPI_INT, 0, 0, MPI_COMM_WORLD, MPI_STATUS_IGNORE);

		// Розпаковує буфер
		size_t index = 0;
		extractFromBuffer(recv11, Z_2H, index, 2 * H);
		extractFromBuffer(recv11, D_2H, index, 2 * H);
		extractFromBuffer(recv11, C_2H, index, 2 * H);
		extractFromBuffer(recv11, MR_2H, index, N, 2 * H);
		extractFromBuffer(recv11, MX, index, N, N);
		extractFromBuffer(recv11, B_2H, index, 2 * H);
		B_H = selectPartsH(B_2H, { 1 });
		Z_H = selectPartsH(Z_2H, { 1 });
		D_H = selectPartsH(D_2H, { 1 });
		C_H = selectPartsH(C_2H, { 1 });
		MR_H = selectPartsH(MR_2H, { 1 });
		auto BH = selectPartsH(B_2H, { 0 });
		auto ZH = selectPartsH(Z_2H, { 0 });
		auto DH = selectPartsH(D_2H, { 0 });
		auto CH = selectPartsH(C_2H, { 0 });
		auto MRH = selectPartsH(MR_2H, { 0 });

		// Формує буфер для відправлення
		vector<int> send71;
		appendToBuffer(send71, ZH);
		appendToBuffer(send71, DH);
		appendToBuffer(send71, CH);
		appendToBuffer(send71, MRH);
		appendToBuffer(send71, MX);
		appendToBuffer(send71, BH);

		// Передає буфер рангу 6 
		MPI_Send(&send71[0], static_cast<int>(send71.size()), MPI_INT, 6, 0, MPI_COMM_WORLD);
	}

	// Обчислення 1
	vector<int> P_H(H);
	for (size_t i = 0; i < H; i++)
	{
		P_H[i] = D_H[i] + Z_H[i];
	}

	// Обчислення 2
	int mi = 0;
	for (size_t i = 0; i < H; i++)
	{
		mi += B_H[i] * C_H[i];
	}

	vector<int> P;
	int m;

	// Передача mi і P_H
	if (rank == 0 || rank == 2 || rank == 3 || rank == 6 || rank == 7)
	{
		// Отримує буфер
		int n_parts = (rank >= 2 && rank <= 3) ? (4 - rank) : (rank + 7) % 8 - 4;
		int rank_from = (rank >= 2 && rank <= 3) ? (rank + 1) : (rank + 7) % 8;
		vector<int> recv_buff(n_parts * H + 1);
		int m_received;
		vector<int> P_received;
		MPI_Recv(recv_buff.data(), static_cast<int>(recv_buff.size()), MPI_INT, rank_from, 0, MPI_COMM_WORLD, MPI_STATUS_IGNORE);

		// Розпаковує буфер
		size_t index = 0;
		extractFromBuffer(recv_buff, m_received, index);
		extractFromBuffer(recv_buff, P_received, index, n_parts * H);

		// Обчислення 3
		mi += m_received;

		// Формує буфер для відправлення
		vector<int> send_buff;
		appendToBuffer(send_buff, mi);
		if (rank <= 4) {
			appendToBuffer(send_buff, P_H);
			appendToBuffer(send_buff, P_received);
		}
		else {
			appendToBuffer(send_buff, P_received);
			appendToBuffer(send_buff, P_H);
		}

		// Передає буфер
		int rank_to = (rank >= 2 && rank <= 3) ? (rank - 1) : (rank + 1) % 8;
		MPI_Send(&send_buff[0], static_cast<int>(send_buff.size()), MPI_INT, rank_to, 0, MPI_COMM_WORLD);
	}
	else if (rank == 4 || rank == 5)
	{
		// Формує буфер для відправлення
		vector<int> send_buff;
		appendToBuffer(send_buff, mi);
		appendToBuffer(send_buff, P_H);

		// Передає буфер
		MPI_Send(&send_buff[0], static_cast<int>(send_buff.size()), MPI_INT, (rank <= 4) ? rank - 1 : rank + 1, 0, MPI_COMM_WORLD);
	}
	else if (rank == 1)
	{
		// Отримує буфери від 0 і 2
		vector<int> recv32(3 * H + 1);
		vector<int> recv12(4 * H + 1);
		int m3, m1;
		vector<int> P_3H, P_4H;
		MPI_Recv(recv32.data(), static_cast<int>(recv32.size()), MPI_INT, 2, 0, MPI_COMM_WORLD, MPI_STATUS_IGNORE);
		MPI_Recv(recv12.data(), static_cast<int>(recv12.size()), MPI_INT, 0, 0, MPI_COMM_WORLD, MPI_STATUS_IGNORE);

		// Розпаковує буфери
		size_t index = 0;
		extractFromBuffer(recv32, m3, index);
		extractFromBuffer(recv32, P_3H, index, 3 * H);
		index = 0;
		extractFromBuffer(recv12, m1, index);
		extractFromBuffer(recv12, P_4H, index, 4 * H);

		// Обчислення 3
		mi += m3;
		mi += m1;
		m = mi;

		// Формує вектор Р з частин
		appendToBuffer(P, selectPartsH(P_4H, { 0 }));
		appendToBuffer(P, P_H);
		appendToBuffer(P, P_3H);
		appendToBuffer(P, selectPartsH(P_4H, { 1, 2, 3 }));
	}

	// Новий комунікатор з рангами 0, 1, 2, щоб виконати колективну операцію
	MPI_Group world_group, custom_group;
	MPI_Comm custom_comm;
	MPI_Comm_group(MPI_COMM_WORLD, &world_group);
	int members[] = { 0, 1, 2 };
	MPI_Group_incl(world_group, 3, members, &custom_group);
	MPI_Comm_create(MPI_COMM_WORLD, custom_group, &custom_comm);

	// Перевірка, чи процес належить до нового комунікатора
	if (custom_comm != MPI_COMM_NULL) {
		int custom_rank;
		MPI_Comm_rank(custom_comm, &custom_rank);

		// Розсилка вектора P і m з процесу 1 до 0 і 2
		P.resize(N);
		MPI_Bcast(&P[0], N, MPI_INT, 1, custom_comm);
		MPI_Bcast(&m, 1, MPI_INT, 1, custom_comm);
	}

	// Очищення комунікатора
	if (custom_group != MPI_GROUP_NULL) MPI_Group_free(&custom_group);
	if (custom_comm != MPI_COMM_NULL) MPI_Comm_free(&custom_comm);
	MPI_Group_free(&world_group);

	// Продовження пересилання готових m i P
	if (rank == 3 || rank == 4 || rank == 5 || rank == 6 || rank == 7)
	{
		// Отримує буфер
		int rank_from = (rank <= 4) ? rank - 1 : (rank + 1) % 8;
		vector<int> recv_buff(N + 1);
		MPI_Recv(recv_buff.data(), static_cast<int>(recv_buff.size()), MPI_INT, rank_from, 0, MPI_COMM_WORLD, MPI_STATUS_IGNORE);

		// Розпаковує буфер
		size_t index = 0;
		extractFromBuffer(recv_buff, m, index);
		extractFromBuffer(recv_buff, P, index, N);
	}
	if (rank == 0 || rank == 2 || rank == 3 || rank == 6 || rank == 7)
	{
		// Формує буфер для відправлення
		int rank_to = (rank == 2 || rank == 3) ? rank + 1 : (rank + 7) % 8;
		vector<int> send_buff;
		appendToBuffer(send_buff, m);
		appendToBuffer(send_buff, P);

		// Передає буфер
		MPI_Send(&send_buff[0], static_cast<int>(send_buff.size()), MPI_INT, rank_to, 0, MPI_COMM_WORLD);
	}

	// Обчислення 4
	vector<int> A_H(H);
	auto MX_MR_H = multiplyMatrices(MX, MR_H);
	auto P_MX_MR_H = multiplyMatrixVector(MX_MR_H, P);

	for (size_t i = 0; i < H; i++)
	{
		A_H[i] = m * Z_H[i] - P_MX_MR_H[i];
	}

	// Пересилання готових A_H
	vector<int> recv_buff;
	if (rank != 4 && rank != 5)
	{
		// Отримує буфер
		int rank_from = (rank >= 1 && rank <= 3) ? rank + 1 : (rank + 7) % 8;
		int n_parts = (rank >= 1 && rank <= 3) ? (4 - rank) : (rank + 7) % 8 - 4;
		vector<int> A_received;
		recv_buff.resize(n_parts * H);
		MPI_Recv(recv_buff.data(), static_cast<int>(recv_buff.size()), MPI_INT, rank_from, 0, MPI_COMM_WORLD, MPI_STATUS_IGNORE);
	}

	if (rank != 1)
	{
		// Формує буфер для відправлення
		int rank_to = (rank >= 2 && rank <= 4) ? rank - 1 : (rank + 1) % 8;
		vector<int> send_buff;

		if (rank <= 4)
		{
			appendToBuffer(send_buff, A_H);
			appendToBuffer(send_buff, recv_buff);
		}
		else
		{
			appendToBuffer(send_buff, recv_buff);
			appendToBuffer(send_buff, A_H);
		}

		// Передає буфер
		MPI_Send(&send_buff[0], static_cast<int>(send_buff.size()), MPI_INT, rank_to, 0, MPI_COMM_WORLD);
	}
	else
	{
		// Отримує буфер
		vector<int> recv13(4 * H);
		MPI_Recv(recv13.data(), static_cast<int>(recv13.size()), MPI_INT, 0, 0, MPI_COMM_WORLD, MPI_STATUS_IGNORE);

		// Складає А з отриманих частин
		vector<int> A;
		appendToBuffer(A, selectPartsH(recv13, { 0 }));
		appendToBuffer(A, A_H);
		appendToBuffer(A, recv_buff);
		appendToBuffer(A, selectPartsH(recv13, { 1, 2, 3 }));

		// Виведення результату
		cout << "Rank: " << rank << " vector A: ";
		for (auto& num : A) {
			cout << num << " ";
		}
		cout << endl;
	}

	MPI_Finalize();
	return 0;
}

void initializeVector(vector<int>& vector) {
	vector.resize(N);
	for (int i = 0; i < N; ++i) {
		vector[i] = 1;
	}
}

void initializeMatrix(vector<vector<int>>& matrix) {
	matrix.resize(N, vector<int>(N));
	for (int i = 0; i < N; ++i) {
		for (int j = 0; j < N; ++j) {
			matrix[i][j] = 1;
		}
	}
}

// Функції для додавання скалярів/векторів/матриць у буфер
void appendToBuffer(vector<int>& buffer, const int& value) {
	buffer.push_back(value);
}

void appendToBuffer(vector<int>& buffer, const vector<int>& array) {
	buffer.insert(buffer.end(), array.begin(), array.end());
}

void appendToBuffer(vector<int>& buffer, const vector<vector<int>>& matrix) {
	for (const auto& row : matrix) {
		appendToBuffer(buffer, row);
	}
}


// Функції для читання скалярів/векторів/матриць з буфера
void extractFromBuffer(vector<int>& buffer, int& value, size_t& index) {
	value = buffer[index++];
}

void extractFromBuffer(vector<int>& buffer, vector<int>& array, size_t& index, size_t size) {
	array.resize(size);
	for (size_t i = 0; i < size; ++i) {
		array[i] = buffer[index++];
	}
}

void extractFromBuffer(vector<int>& buffer, vector<vector<int>>& matrix, size_t& index, size_t rows, size_t cols) {
	matrix.resize(rows, vector<int>(cols));
	for (size_t i = 0; i < rows; ++i) {
		for (size_t j = 0; j < cols; ++j) {
			matrix[i][j] = buffer[index++];
		}
	}
}

// Фільтрація частин вектора/матриці для передачі
vector<int> selectPartsH(const vector<int>& array, const vector<int>& parts) {
	vector<int> selectedParts;

	for (int part : parts) {
		for (int i = part * H; i < (part + 1) * H; ++i) {
			selectedParts.push_back(array[i]);
		}
	}

	return selectedParts;
}

vector<vector<int>> selectPartsH(const vector<vector<int>>& matrix, const vector<int>& parts) {
	vector<vector<int>> selectedParts;

	for (int i = 0; i < N; ++i) {
		vector<int> row;
		for (int part : parts) {
			for (int j = part * H; j < (part + 1) * H; ++j) {
				row.push_back(matrix[i][j]);
			}
		}
		selectedParts.push_back(row);
	}

	return selectedParts;
}

// Функція для множення двох матриць
vector<vector<int>> multiplyMatrices(const vector<vector<int>>& matrix1, const vector<vector<int>>& matrix2) {
	size_t rows1 = matrix1.size();
	size_t cols1 = matrix1[0].size();
	size_t cols2 = matrix2[0].size();
	vector<vector<int>> result(rows1, vector<int>(cols2, 0));
	for (size_t i = 0; i < rows1; ++i) {
		for (size_t j = 0; j < cols2; ++j) {
			for (size_t k = 0; k < cols1; ++k) {
				result[i][j] += matrix1[i][k] * matrix2[k][j];
			}
		}
	}
	return result;
}

// Функція для множення вектора на матрицю
vector<int> multiplyMatrixVector(const vector<vector<int>>& matrix, const vector<int>& vec) {
	auto n = vec.size();
	auto m = matrix[0].size();

	vector<int> result(m);

	for (int i = 0; i < m; i++) {
		result[i] = 0;
		for (int j = 0; j < n; j++) {
			result[i] += vec[j] * matrix[j][i];
		}
	}

	return result;
}